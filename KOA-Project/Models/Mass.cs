using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOA_Project.Models;

[Table("mass_weekdays")]
public class MassWeekday
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("day")]
    [MaxLength(20)]
    public string Day { get; set; } = string.Empty;

    [Column("mass_type")]
    [MaxLength(5)]
    public string MassType { get; set; } = "AM";

    [Column("mass_time")]
    public TimeOnly MassTime { get; set; }

    [Column("description")]
    [MaxLength(255)]
    public string? Description { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

[Table("mass_sundays")]
public class MassSunday
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("mass_name")]
    [MaxLength(255)]
    public string MassName { get; set; } = "Sunday Mass";

    [Column("mass_time")]
    public TimeOnly MassTime { get; set; }

    [Column("description")]
    [MaxLength(255)]
    public string? Description { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Active";

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
