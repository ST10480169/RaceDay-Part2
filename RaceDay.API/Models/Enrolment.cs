using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

[Table("Enrolments", Schema = "dbo")]
public class Enrolment
{
    [Key] public int EnrolmentID { get; set; }
    public int ParticipantID { get; set; }
    public int CategoryID { get; set; }
    public DateTime EnrolmentDate { get; set; }
    [MaxLength(10)] public string? BibNumber { get; set; }
    [Required, MaxLength(20)] public string Status { get; set; } = "Confirmed";
    public User? Participant { get; set; }
    public Category? Category { get; set; }
    public Result? Result { get; set; }
}
