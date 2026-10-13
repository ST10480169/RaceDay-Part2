using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController(RaceDayDbContext db) : ControllerBase
{
    /// <summary>Lists upcoming events, including venue and categories.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetEvents() => Ok(await db.Events.AsNoTracking().Where(e => e.EventDate >= DateTime.Today)
        .Include(e => e.Venue).Include(e => e.Categories).OrderBy(e => e.EventDate).ToListAsync());

    /// <summary>Returns event details, venue, and categories.</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetEvent(int id)
    {
        var item = await db.Events.AsNoTracking().Include(e => e.Venue).Include(e => e.Categories).SingleOrDefaultAsync(e => e.EventID == id);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>Creates an event owned by the logged-in Organiser.</summary>
    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(EventRequest request)
    {
        if (!await db.Venues.AnyAsync(v => v.VenueID == request.VenueId)) return BadRequest(new { message = "Venue does not exist." });
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(id, out var organiserId)) return Unauthorized();
        var item = new Event { Title = request.Title.Trim(), Description = request.Description, EventDate = request.EventDate.Date,
            EventType = request.EventType, VenueID = request.VenueId, OrganiserID = organiserId, CreatedAt = DateTime.Now };
        db.Events.Add(item); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEvent), new { id = item.EventID }, item);
    }

    /// <summary>Updates an event only when the logged-in Organiser owns it.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, EventRequest request)
    {
        var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var item = await db.Events.SingleOrDefaultAsync(e => e.EventID == id);
        if (item is null) return NotFound();
        if (item.OrganiserID != ownerId) return Forbid();
        if (!await db.Venues.AnyAsync(v => v.VenueID == request.VenueId)) return BadRequest(new { message = "Venue does not exist." });
        item.Title = request.Title.Trim(); item.Description = request.Description; item.EventDate = request.EventDate.Date;
        item.EventType = request.EventType; item.VenueID = request.VenueId;
        await db.SaveChangesAsync(); return Ok(item);
    }

    /// <summary>Deletes an event only when the logged-in Organiser owns it.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var item = await db.Events.SingleOrDefaultAsync(e => e.EventID == id);
        if (item is null) return NotFound();
        if (item.OrganiserID != ownerId) return Forbid();
        db.Events.Remove(item); await db.SaveChangesAsync(); return NoContent();
    }
}
