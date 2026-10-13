using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController]
public class CategoriesController(RaceDayDbContext db) : ControllerBase
{
    /// <summary>Lists categories for an event.</summary>
    [HttpGet("api/events/{eventId:int}/categories")]
    [AllowAnonymous]
    public async Task<IActionResult> GetForEvent(int eventId)
    {
        if (!await db.Events.AnyAsync(e => e.EventID == eventId)) return NotFound();
        return Ok(await db.Categories.AsNoTracking().Where(c => c.EventID == eventId).OrderBy(c => c.DistanceKm).ToListAsync());
    }

    /// <summary>Creates a category under an event owned by the logged-in Organiser.</summary>
    [HttpPost("api/events/{eventId:int}/categories")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(int eventId, CategoryRequest request)
    {
        var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var item = await db.Events.SingleOrDefaultAsync(e => e.EventID == eventId);
        if (item is null) return NotFound();
        if (item.OrganiserID != ownerId) return Forbid();
        var category = new Category { EventID = eventId, Name = request.Name.Trim(), DistanceKm = request.DistanceKm,
            EntryFee = request.EntryFee, MaxParticipants = request.MaxParticipants };
        db.Categories.Add(category); await db.SaveChangesAsync();
        return Created($"/api/events/{eventId}/categories", category);
    }

    /// <summary>Updates a category only when its event belongs to the logged-in Organiser.</summary>
    [HttpPut("api/categories/{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, CategoryRequest request)
    {
        var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var category = await db.Categories.Include(c => c.Event).SingleOrDefaultAsync(c => c.CategoryID == id);
        if (category is null) return NotFound();
        if (category.Event!.OrganiserID != ownerId) return Forbid();
        category.Name = request.Name.Trim(); category.DistanceKm = request.DistanceKm; category.EntryFee = request.EntryFee; category.MaxParticipants = request.MaxParticipants;
        await db.SaveChangesAsync(); return Ok(category);
    }

    /// <summary>Deletes a category only when its event belongs to the logged-in Organiser.</summary>
    [HttpDelete("api/categories/{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var category = await db.Categories.Include(c => c.Event).SingleOrDefaultAsync(c => c.CategoryID == id);
        if (category is null) return NotFound();
        if (category.Event!.OrganiserID != ownerId) return Forbid();
        db.Categories.Remove(category); await db.SaveChangesAsync(); return NoContent();
    }
}
