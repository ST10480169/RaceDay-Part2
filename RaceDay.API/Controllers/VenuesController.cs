using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/venues")]
public class VenuesController(RaceDayDbContext db) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetVenues()
    {
        var venues = await db.Venues
            .AsNoTracking()
            .OrderBy(v => v.Name)
            .ToListAsync();

        return Ok(venues);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetVenue(int id)
    {
        var venue = await db.Venues
            .AsNoTracking()
            .SingleOrDefaultAsync(v => v.VenueID == id);

        return venue is null ? NotFound() : Ok(venue);
    }

    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> CreateVenue(Venue venue)
    {
        venue.VenueID = 0;

        db.Venues.Add(venue);
        await db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetVenue),
            new { id = venue.VenueID },
            venue);
    }
}