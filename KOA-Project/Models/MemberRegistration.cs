using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KOA_Project.Models;

[Table("member_registrations")]
public class MemberRegistration
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("first_name")]
    [MaxLength(255)]
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Column("middle_name")]
    [MaxLength(255)]
    public string? MiddleName { get; set; }

    [Column("last_name")]
    [MaxLength(255)]
    [Required]
    public string LastName { get; set; } = string.Empty;

    [Column("birth_date")]
    public DateOnly? BirthDate { get; set; }

    [Column("contact_number")]
    [MaxLength(25)]
    public string? ContactNumber { get; set; }

    [Column("address")]
    [MaxLength(500)]
    public string? Address { get; set; }

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

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
    public string Initials => $"{(FirstName.Length > 0 ? FirstName[0] : ' ')}{(LastName.Length > 0 ? LastName[0] : ' ')}".ToUpper();
}