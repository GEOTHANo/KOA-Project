using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOA_Project.Models;

[Table("positions")]
public class Position
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("position_name")]
    [MaxLength(255)]
    public string PositionName { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<MemberPosition> MemberPositions { get; set; } = new List<MemberPosition>();
}
