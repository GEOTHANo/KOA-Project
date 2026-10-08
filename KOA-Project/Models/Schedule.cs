using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOA_Project.Models;

[Table("schedule_weekdays")]
public class ScheduleWeekday
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("week_start")]
    public DateOnly WeekStart { get; set; }

    [Column("week_end")]
    public DateOnly WeekEnd { get; set; }

    [Column("day")]
    [MaxLength(20)]
    public string Day { get; set; } = string.Empty;

    [Column("mass_type")]
    [MaxLength(5)]
    public string MassType { get; set; } = "AM";

    [Column("mass_time")]
    public TimeOnly MassTime { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "not checked";

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<WeekdayScheduleMember> ScheduleMembers { get; set; } = new List<WeekdayScheduleMember>();
}

[Table("schedule_sunday")]
public class ScheduleSunday
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("mass_date")]
    public DateOnly MassDate { get; set; }

    [Column("mass_name")]
    [MaxLength(255)]
    public string MassName { get; set; } = "Sunday Mass";

    [Column("mass_time")]
    public TimeOnly MassTime { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "not checked";

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<SundayScheduleMember> ScheduleMembers { get; set; } = new List<SundayScheduleMember>();
}
