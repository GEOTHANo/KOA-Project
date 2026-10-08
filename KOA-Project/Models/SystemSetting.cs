using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOA_Project.Models;

[Table("system_settings")]
public class SystemSetting
{
    [Key]
    [Column("key_name")]
    [MaxLength(100)]
    public string KeyName { get; set; } = string.Empty;

    [Column("value")]
    public string? Value { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
