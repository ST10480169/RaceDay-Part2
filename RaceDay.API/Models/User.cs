using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

[Table("Users", Schema = "dbo")]
public class User
{
    [Key] public int UserID { get; set; }
    [Required, MaxLength(100)] public string FullName { get; set; } = string.Empty;
    [Required, MaxLength(150), EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, MaxLength(255)] public string PasswordHash { get; set; } = string.Empty;
    [Required, MaxLength(20)] public string Role { get; set; } = "Participant";
    [MaxLength(20)] public string? PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Event> OrganisedEvents { get; set; } = new List<Event>();
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}
