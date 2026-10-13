using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

[Table("Events", Schema = "dbo")]
public class Event
{
    [Key] public int EventID { get; set; }
    public int OrganiserID { get; set; }
    public int VenueID { get; set; }
    [Required, MaxLength(150)] public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Column(TypeName = "date")] public DateTime EventDate { get; set; }
    [Required, MaxLength(20)] public string EventType { get; set; } = "Running";
    public DateTime CreatedAt { get; set; }
    public User? Organiser { get; set; }
    public Venue? Venue { get; set; }
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}
