using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

[Table("Venues", Schema = "dbo")]
public class Venue
{
    [Key] public int VenueID { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(200)] public string? AddressLine { get; set; }
    [Required, MaxLength(100)] public string City { get; set; } = string.Empty;
    [MaxLength(100)] public string? Province { get; set; }
    [Column(TypeName = "decimal(9,6)")] public decimal? Latitude { get; set; }
    [Column(TypeName = "decimal(9,6)")] public decimal? Longitude { get; set; }
    public ICollection<Event> Events { get; set; } = new List<Event>();
}
