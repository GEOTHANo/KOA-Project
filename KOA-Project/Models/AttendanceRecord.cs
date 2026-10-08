using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOA_Project.Models;

[Table("attendance")]
public class AttendanceRecord
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("member_id")]
    public long MemberId { get; set; }

    [Column("meeting_week_start")]
    public DateOnly MeetingWeekStart { get; set; }

    [Column("meeting_week_end")]
    public DateOnly MeetingWeekEnd { get; set; }

    [Column("attendance_date")]
    public DateOnly AttendanceDate { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Absent";

    [Column("remarks")]
    public string? Remarks { get; set; }

    [Column("recorded_by")]
    public long? RecordedBy { get; set; }

    [Column("date_recorded")]
    public DateTime DateRecorded { get; set; } = DateTime.Now;

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    [ForeignKey("MemberId")]
    public Member Member { get; set; } = null!;

    [ForeignKey("RecordedBy")]
    public Member? RecordedByMember { get; set; }
}
