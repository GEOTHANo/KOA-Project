using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOA_Project.Models;

[Table("member_positions")]
public class MemberPosition
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("member_id")]
    public long MemberId { get; set; }

    [Column("position_id")]
    public long PositionId { get; set; }

    [Column("date_assigned")]
    public DateOnly DateAssigned { get; set; }

    [Column("is_current")]
    public bool IsCurrent { get; set; } = true;

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    [ForeignKey("MemberId")]
    public Member Member { get; set; } = null!;

    [ForeignKey("PositionId")]
    public Position Position { get; set; } = null!;
}
