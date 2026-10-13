using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

[Table("Results", Schema = "dbo")]
public class Result
{
    [Key] public int ResultID { get; set; }
    public int EnrolmentID { get; set; }
    [Column(TypeName = "time")] public TimeSpan? FinishTime { get; set; }
    public int? Position { get; set; }
    [Required, MaxLength(20)] public string Status { get; set; } = "Finished";
    public Enrolment? Enrolment { get; set; }
}
