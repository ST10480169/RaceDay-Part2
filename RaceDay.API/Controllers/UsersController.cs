using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(RaceDayDbContext db) : ControllerBase
{
    /// <summary>Returns the authenticated user's own profile without exposing the password hash.</summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> Me()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(id, out var userId)) return Unauthorized();
        var user = await db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.UserID == userId);
        return user is null ? NotFound() : Ok(new UserResponse(user.UserID, user.FullName, user.Email, user.Role, user.PhoneNumber, user.CreatedAt));
    }

    /// <summary>Updates the authenticated user's own name and phone number.</summary>
    [HttpPut("me")]
    public async Task<ActionResult<UserResponse>> UpdateMe(ProfileUpdateRequest request)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(id, out var userId)) return Unauthorized();
        var user = await db.Users.SingleOrDefaultAsync(x => x.UserID == userId);
        if (user is null) return NotFound();
        user.FullName = request.FullName.Trim(); user.PhoneNumber = request.PhoneNumber?.Trim();
        await db.SaveChangesAsync();
        return Ok(new UserResponse(user.UserID, user.FullName, user.Email, user.Role, user.PhoneNumber, user.CreatedAt));
    }

    /// <summary>Allows an Organiser to view a user's public profile.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<ActionResult<UserResponse>> GetUser(int id)
    {
        var user = await db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.UserID == id);
        return user is null ? NotFound() : Ok(new UserResponse(user.UserID, user.FullName, user.Email, user.Role, user.PhoneNumber, user.CreatedAt));
    }
}
