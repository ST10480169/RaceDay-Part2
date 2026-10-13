using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs;

public sealed class EventRequest
{
    [Required, MaxLength(150)] public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    [Required] public DateTime EventDate { get; init; }
    [Required, RegularExpression("^(Running|Walking|Cycling)$")] public string EventType { get; init; } = "Running";
    [Range(1, int.MaxValue)] public int VenueId { get; init; }
}

public sealed class ProfileUpdateRequest
{
    [Required, MaxLength(100)] public string FullName { get; init; } = string.Empty;
    [MaxLength(20)] public string? PhoneNumber { get; init; }
}

public sealed class CategoryRequest
{
    [Required, MaxLength(50)] public string Name { get; init; } = string.Empty;
    [Range(typeof(decimal), "0.01", "999.99")] public decimal DistanceKm { get; init; }
    [Range(typeof(decimal), "0", "999999.99")] public decimal EntryFee { get; init; }
    [Range(1, 100000)] public int MaxParticipants { get; init; } = 100;
}

public sealed class ResultRequest
{
    public TimeSpan? FinishTime { get; init; }
    [Range(1, int.MaxValue)] public int? Position { get; init; }
    [Required, RegularExpression("^(Finished|DNF|DNS)$")] public string Status { get; init; } = "Finished";
}
