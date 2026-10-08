using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOA_Project.Models;

[Table("weekday_schedule_members")]
public class WeekdayScheduleMember
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("weekday_schedule_id")]
    public long WeekdayScheduleId { get; set; }

    [Column("member_id")]
    public long MemberId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("assigned_date")]
    public DateOnly AssignedDate { get; set; }

    [Column("attendance_status")]
    [MaxLength(20)]
    public string? AttendanceStatus { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    [ForeignKey("WeekdayScheduleId")]
    public ScheduleWeekday Schedule { get; set; } = null!;

    [ForeignKey("MemberId")]
    public Member Member { get; set; } = null!;
}

[Table("sunday_schedule_members")]
public class SundayScheduleMember
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("sunday_schedule_id")]
    public long SundayScheduleId { get; set; }

    [Column("member_id")]
    public long MemberId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("assigned_date")]
    public DateOnly AssignedDate { get; set; }

    [Column("attendance_status")]
    [MaxLength(20)]
    public string? AttendanceStatus { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    [ForeignKey("SundayScheduleId")]
    public ScheduleSunday Schedule { get; set; } = null!;

    [ForeignKey("MemberId")]
    public Member Member { get; set; } = null!;
}
