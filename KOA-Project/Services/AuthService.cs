using System.Security.Claims;
using KOA_Project.Data;
using KOA_Project.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace KOA_Project.Services;

public class AuthService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IDbContextFactory<ApplicationDbContext> dbFactory, IHttpContextAccessor httpContextAccessor)
    {
        _dbFactory = dbFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<(bool Success, string? Error)> LoginAsync(HttpContext context, string username, string password)
    {
        using var db = await _dbFactory.CreateDbContextAsync();

        var cleanUsername = username?.Trim() ?? string.Empty;
        var member = await db.Members
            .Include(m => m.MemberPositions.Where(mp => mp.IsCurrent))
            .ThenInclude(mp => mp.Position)
            .FirstOrDefaultAsync(m => m.Username.ToLower() == cleanUsername.ToLower());

        if (member == null)
            return (false, "The provided credentials do not match our records.");

        // Check if account is active (DB enum: Active, Inactive, Alumni - no Locked)
        if (member.Status != "Active")
            return (false, "This account is inactive. Please contact an administrator.");
        
        // Check if account is locked due to failed attempts
        if (member.FailedLoginAttempts >= 10)
            return (false, "Your account has been locked after 10 failed login attempts. Please contact an administrator.");

        // Verify bcrypt password (set by Laravel)
        bool valid;
        try
        {
            valid = BCrypt.Net.BCrypt.Verify(password, member.Password);
        }
        catch
        {
            valid = false;
        }

        if (!valid)
        {
            // Increment failed attempts
            member.FailedLoginAttempts += 1;
            member.UpdatedAt = DateTime.UtcNow;

            if (member.FailedLoginAttempts >= 10)
            {
                await db.SaveChangesAsync();
                return (false, "Your account has been locked after 10 failed login attempts. Please contact an administrator to unlock it.");
            }

            int remaining = 10 - member.FailedLoginAttempts;
            await db.SaveChangesAsync();
            return (false, $"Incorrect password. You have {remaining} attempt{(remaining == 1 ? "" : "s")} remaining before your account is locked.");
        }

        // Successful login — reset failed attempts
        member.FailedLoginAttempts = 0;
        member.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        // Build claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, member.Id.ToString()),
            new(ClaimTypes.Name, member.Username),
            new("FirstName", member.FirstName),
            new("LastName", member.LastName),
            new("FullName", member.FullName),
        };

        // Add position names as roles
        foreach (var mp in member.MemberPositions.Where(p => p.IsCurrent))
        {
            claims.Add(new Claim(ClaimTypes.Role, mp.Position.PositionName));
            claims.Add(new Claim("PositionId", mp.PositionId.ToString()));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                AllowRefresh = true
            });

        // Log login
        try
        {
            db.UserLogs.Add(new UserLog
            {
                MemberId = member.Id,
                LoginTime = DateTime.UtcNow,
                IpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }
        catch { /* non-critical */ }

        return (true, null);
    }

    public async Task LogoutAsync()
    {
        if (_httpContextAccessor.HttpContext != null)
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    // Permission logic mirroring Laravel Member::canAccess()
    public static bool CanAccess(ClaimsPrincipal user, string area)
    {
        if (!user.Identity?.IsAuthenticated ?? true) return false;

        var positions = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

        if (!positions.Any()) return false;

        var superRoles = new[] { "Coordinator", "Vice Coordinator", "Arts Chairman" };
        if (positions.Any(p => superRoles.Contains(p))) return true;

        var permissions = new Dictionary<string, string[]>
        {
            ["Secretary"]             = new[] { "dashboard", "schedules", "schedules.create", "members", "attendance", "notifications", "notifications.create", "settings", "mass" },
            ["Assistant Secretary"]   = new[] { "dashboard", "schedules", "schedules.create", "members", "attendance", "notifications", "notifications.create", "settings" },
            ["Treasurer"]             = new[] { "dashboard", "schedules", "notifications", "settings" },
            ["Assistant Treasurer"]   = new[] { "dashboard", "schedules", "notifications", "settings" },
            ["Socio-Cultural Chairman"] = new[] { "dashboard", "schedules", "notifications", "settings" },
            ["Spirituality Chairman"] = new[] { "schedules", "notifications", "settings" },
            ["Sports Chairman"]       = new[] { "schedules", "notifications", "settings" },
            ["Music Chairman"]        = new[] { "schedules", "notifications", "settings" },
            ["Companion Brother"]     = new[] { "dashboard", "schedules", "notifications", "settings" },
            ["Member"]                = new[] { "member-dashboard", "notifications", "settings" },
        };

        var allowed = positions
            .Where(p => permissions.ContainsKey(p))
            .SelectMany(p => permissions[p])
            .ToHashSet();

        return allowed.Contains(area);
    }
}

