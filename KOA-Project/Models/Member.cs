using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOA_Project.Models;

[Table("members")]
public class Member
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("username")]
    [MaxLength(255)]
    public string Username { get; set; } = string.Empty;

    [Column("password")]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    [Column("first_name")]
    [MaxLength(255)]
    public string FirstName { get; set; } = string.Empty;

    [Column("middle_name")]
    [MaxLength(255)]
    public string? MiddleName { get; set; }

    [Column("last_name")]
    [MaxLength(255)]
    public string LastName { get; set; } = string.Empty;

    [Column("birth_date")]
    public DateOnly? BirthDate { get; set; }

    [Column("contact_number")]
    [MaxLength(25)]
    public string? ContactNumber { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    [Column("failed_login_attempts")]
    public int FailedLoginAttempts { get; set; } = 0;

    [Column("remember_token")]
    [MaxLength(100)]
    public string? RememberToken { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    // Computed properties
    [NotMapped]
    public string FullName
    {
        get
        {
            var parts = new[] { FirstName, MiddleName, LastName }
                .Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join(" ", parts);
        }
    }

    [NotMapped]
    public string Initials => $"{(!string.IsNullOrEmpty(FirstName) ? FirstName[0] : ' ')}{(!string.IsNullOrEmpty(LastName) ? LastName[0] : ' ')}".Trim().ToUpper();

    // Navigation properties
    public ICollection<MemberPosition> MemberPositions { get; set; } = new List<MemberPosition>();
    public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    public ICollection<AttendanceRecord> RecordedAttendances { get; set; } = new List<AttendanceRecord>();
    public ICollection<WeekdayScheduleMember> WeekdayScheduleMembers { get; set; } = new List<WeekdayScheduleMember>();
    public ICollection<SundayScheduleMember> SundayScheduleMembers { get; set; } = new List<SundayScheduleMember>();
    public ICollection<UserLog> UserLogs { get; set; } = new List<UserLog>();
    public ICollection<Notification> CreatedNotifications { get; set; } = new List<Notification>();
}
