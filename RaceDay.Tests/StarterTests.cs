using Microsoft.AspNetCore.Identity;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.Tests;

public class StarterTests
{
    [Fact]
    public void PasswordHasher_StoresVerifiableHash_NotPlainText()
    {
        var user = new User { Email = "test@example.com", FullName = "Test User", Role = "Participant" };
        var hasher = new PasswordHasher<User>();
        var hash = hasher.HashPassword(user, "Example-Password-123!");
        Assert.NotEqual("Example-Password-123!", hash);
        Assert.Equal(PasswordVerificationResult.Success, hasher.VerifyHashedPassword(user, hash, "Example-Password-123!"));
        Assert.Equal(PasswordVerificationResult.Failed, hasher.VerifyHashedPassword(user, hash, "wrong-password"));
    }

    [Theory]
    [InlineData("Organiser", true)]
    [InlineData("Participant", true)]
    [InlineData("Admin", false)]
    [InlineData("", false)]
    public void RegisterRequest_OnlyAllowsTheTwoDefinedRoles(string role, bool expectedValid)
    {
        var request = new RegisterRequest { FullName = "Test User", Email = "test@example.com", Password = "Example-Password-123!", Role = role };
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(request);
        var errors = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(request, context, errors, validateAllProperties: true);
        Assert.Equal(expectedValid, isValid);
    }
}
