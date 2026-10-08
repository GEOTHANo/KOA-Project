using KOA_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace KOA_Project.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Member> Members => Set<Member>();
    public DbSet<MemberRegistration> MemberRegistrations => Set<MemberRegistration>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<MemberPosition> MemberPositions => Set<MemberPosition>();
    public DbSet<AttendanceRecord> Attendances => Set<AttendanceRecord>();
    public DbSet<MassWeekday> MassWeekdays => Set<MassWeekday>();
    public DbSet<MassSunday> MassSundays => Set<MassSunday>();
    public DbSet<ScheduleWeekday> ScheduleWeekdays => Set<ScheduleWeekday>();
    public DbSet<ScheduleSunday> ScheduleSundays => Set<ScheduleSunday>();
    public DbSet<WeekdayScheduleMember> WeekdayScheduleMembers => Set<WeekdayScheduleMember>();
    public DbSet<SundayScheduleMember> SundayScheduleMembers => Set<SundayScheduleMember>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<UserLog> UserLogs => Set<UserLog>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Member - soft delete filter
        modelBuilder.Entity<Member>().HasQueryFilter(m => m.DeletedAt == null);

        // Member + MemberPositions (one-to-many)
        modelBuilder.Entity<MemberPosition>()
            .HasOne(mp => mp.Member)
            .WithMany(m => m.MemberPositions)
            .HasForeignKey(mp => mp.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MemberPosition>()
            .HasOne(mp => mp.Position)
            .WithMany(p => p.MemberPositions)
            .HasForeignKey(mp => mp.PositionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Attendance - recorded_by is a second FK to members, avoid cascade conflicts
        modelBuilder.Entity<AttendanceRecord>()
            .HasOne(a => a.Member)
            .WithMany(m => m.AttendanceRecords)
            .HasForeignKey(a => a.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AttendanceRecord>()
            .HasOne(a => a.RecordedByMember)
            .WithMany(m => m.RecordedAttendances)
            .HasForeignKey(a => a.RecordedBy)
            .OnDelete(DeleteBehavior.NoAction);

        // WeekdayScheduleMember
        modelBuilder.Entity<WeekdayScheduleMember>()
            .HasOne(w => w.Schedule)
            .WithMany(s => s.ScheduleMembers)
            .HasForeignKey(w => w.WeekdayScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WeekdayScheduleMember>()
            .HasOne(w => w.Member)
            .WithMany(m => m.WeekdayScheduleMembers)
            .HasForeignKey(w => w.MemberId)
            .OnDelete(DeleteBehavior.NoAction);

        // SundayScheduleMember
        modelBuilder.Entity<SundayScheduleMember>()
            .HasOne(s => s.Schedule)
            .WithMany(sc => sc.ScheduleMembers)
            .HasForeignKey(s => s.SundayScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SundayScheduleMember>()
            .HasOne(s => s.Member)
            .WithMany(m => m.SundayScheduleMembers)
            .HasForeignKey(s => s.MemberId)
            .OnDelete(DeleteBehavior.NoAction);

        // Notification + Creator
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Creator)
            .WithMany(m => m.CreatedNotifications)
            .HasForeignKey(n => n.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);

        // UserLog
        modelBuilder.Entity<UserLog>()
            .HasOne(ul => ul.Member)
            .WithMany(m => m.UserLogs)
            .HasForeignKey(ul => ul.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
