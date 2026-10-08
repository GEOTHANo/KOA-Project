using System.Text.Json;
using System.Text.Json.Serialization;

namespace KOA_Project.Services;

/// <summary>Model for a single liturgical event from the LitCal API.</summary>
public class LiturgicalEvent
{
    [JsonPropertyName("event_key")]  public string EventKey  { get; set; } = "";
    [JsonPropertyName("name")]       public string Name      { get; set; } = "";
    [JsonPropertyName("color")]      public List<string> Color { get; set; } = new();
    [JsonPropertyName("grade")]      public int Grade { get; set; }
    [JsonPropertyName("grade_lcl")]  public string GradeLcl  { get; set; } = "";
    [JsonPropertyName("date")]       public DateTime Date    { get; set; }
    [JsonPropertyName("liturgical_season")]     public string? LiturgicalSeason    { get; set; }
    [JsonPropertyName("liturgical_season_lcl")] public string? LiturgicalSeasonLcl { get; set; }
    [JsonPropertyName("holy_day_of_obligation")] public bool? HolyDayOfObligation { get; set; }
    [JsonPropertyName("is_vigil_mass")]          public bool? IsVigilMass          { get; set; }
    [JsonPropertyName("readings")]               public JsonElement? Readings       { get; set; }

    // Derived helpers
    public string PrimaryColor => Color.FirstOrDefault() ?? "green";

    public string SeasonDisplay => LiturgicalSeason switch
    {
        "ADVENT"        => "Advent",
        "CHRISTMAS"     => "Christmas",
        "LENT"          => "Lent",
        "EASTER"        => "Easter",
        "ORDINARY_TIME" => "Ordinary Time",
        _               => LiturgicalSeason ?? ""
    };

    public string GradeDisplay => Grade switch
    {
        7 => "Higher-Ranking Solemnity",
        6 => "Solemnity",
        5 => "Feast of the Lord",
        4 => "Feast",
        3 => "Obligatory Memorial",
        2 => "Optional Memorial",
        1 => "Commemoration",
        _ => "Feria"
    };

    /// <summary>Returns simple readings as a dictionary (null when complex multi-Mass structure).</summary>
    public Dictionary<string, string>? GetSimpleReadings()
    {
        if (Readings == null) return null;
        var el = Readings.Value;
        if (el.ValueKind != JsonValueKind.Object) return null;

        // Check if it has nested Mass objects (Christmas) vs flat readings
        if (el.TryGetProperty("night", out _) || el.TryGetProperty("day", out _)) return null;

        var dict = new Dictionary<string, string>();
        foreach (var prop in el.EnumerateObject())
        {
            if (prop.Value.ValueKind == JsonValueKind.String)
            {
                var val = prop.Value.GetString();
                if (!string.IsNullOrWhiteSpace(val)) dict[prop.Name] = val;
            }
        }
        return dict.Any() ? dict : null;
    }
}

public class LitCalApiResponse
{
    [JsonPropertyName("litcal")] public List<LiturgicalEvent> LitCal { get; set; } = new();
}

/// <summary>
/// Service that fetches and caches the Liturgical Calendar for the current year.
/// Caches for 24 hours (new data is fetched once per day).
/// </summary>
public class LiturgicalCalendarService
{
    private readonly HttpClient _http;
    private readonly ILogger<LiturgicalCalendarService> _logger;

    // Simple static cache — good for Blazor Server singleton lifetime
    private static List<LiturgicalEvent> _cachedEvents = new();
    private static int _cachedYear = 0;
    private static DateTime _cacheExpiry = DateTime.MinValue;

    private static readonly SemaphoreSlim _lock = new(1, 1);

    public LiturgicalCalendarService(HttpClient http, ILogger<LiturgicalCalendarService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<LiturgicalEvent?> GetTodayAsync()
    {
        var today = DateTime.Now.Date;
        var events = await GetYearAsync(today.Year);

        // Find the highest-grade event for today (skip vigil masses displayed on prior day)
        return events
            .Where(e => e.Date.Date == today && e.IsVigilMass != true)
            .OrderByDescending(e => e.Grade)
            .FirstOrDefault();
    }

    private async Task<List<LiturgicalEvent>> GetYearAsync(int year)
    {
        // Return cache if still valid and for the same year
        if (year == _cachedYear && DateTime.Now < _cacheExpiry && _cachedEvents.Any())
            return _cachedEvents;

        await _lock.WaitAsync();
        try
        {
            // Double-check after acquiring lock
            if (year == _cachedYear && DateTime.Now < _cacheExpiry && _cachedEvents.Any())
                return _cachedEvents;

            var url = $"https://litcal.johnromanodorazio.com/api/v5/calendar/{year}?locale=en_US";
            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<LitCalApiResponse>(json, options);

            if (result?.LitCal != null)
            {
                _cachedEvents = result.LitCal;
                _cachedYear = year;
                _cacheExpiry = DateTime.Now.AddHours(24);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch liturgical calendar from API");
        }
        finally
        {
            _lock.Release();
        }

        return _cachedEvents;
    }
}
