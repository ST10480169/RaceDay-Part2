using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs;

public sealed class RegisterRequest
{
    [Required, MaxLength(100)] public string FullName { get; init; } = string.Empty;
    [Required, EmailAddress, MaxLength(150)] public string Email { get; init; } = string.Empty;
    [Required, MinLength(8), MaxLength(100)] public string Password { get; init; } = string.Empty;
    [Required, RegularExpression("^(Organiser|Participant)$")] public string Role { get; init; } = "Participant";
    [MaxLength(20)] public string? PhoneNumber { get; init; }
}

public sealed class LoginRequest
{
    [Required, EmailAddress] public string Email { get; init; } = string.Empty;
    [Required] public string Password { get; init; } = string.Empty;
}

public sealed record UserResponse(int UserID, string FullName, string Email, string Role, string? PhoneNumber, DateTime CreatedAt);
public sealed record LoginResponse(string Token, string Role, int UserId);
