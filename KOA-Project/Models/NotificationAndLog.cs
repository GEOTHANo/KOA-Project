using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOA_Project.Models;

[Table("notifications")]
public class Notification
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("title")]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [Column("message")]
    public string Message { get; set; } = string.Empty;

    [Column("target_roles")]
    public string? TargetRoles { get; set; } // JSON stored as string

    [Column("created_by")]
    public long? CreatedBy { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    [ForeignKey("CreatedBy")]
    public Member? Creator { get; set; }

    // Helper to get/set target role IDs as a list
    [NotMapped]
    public List<long> TargetRoleIds
    {
        get => string.IsNullOrEmpty(TargetRoles)
            ? new List<long>()
            : System.Text.Json.JsonSerializer.Deserialize<List<long>>(TargetRoles) ?? new List<long>();
        set => TargetRoles = System.Text.Json.JsonSerializer.Serialize(value);
    }

    [Column("read_by")]
    public string? ReadBy { get; set; } // JSON stored as string

    // Helper to get/set read user IDs as a list
    [NotMapped]
    public List<long> ReadByIds
    {
        get => string.IsNullOrEmpty(ReadBy)
            ? new List<long>()
            : System.Text.Json.JsonSerializer.Deserialize<List<long>>(ReadBy) ?? new List<long>();
        set => ReadBy = System.Text.Json.JsonSerializer.Serialize(value);
    }
}

[Table("user_logs")]
public class UserLog
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("member_id")]
    public long MemberId { get; set; }

    [Column("login_time")]
    public DateTime LoginTime { get; set; }

    [Column("ip_address")]
    [MaxLength(45)]
    public string? IpAddress { get; set; }

    [Column("user_agent")]
    public string? UserAgent { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("MemberId")]
    public Member Member { get; set; } = null!;
}
