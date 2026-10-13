using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api")]
public class ResultsController(RaceDayDbContext db) : ControllerBase
{
    /// <summary>Captures a result for an enrolment in an event owned by the Organiser.</summary>
    [HttpPost("enrolments/{enrolmentId:int}/results")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(int enrolmentId, ResultRequest request)
    {
        var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var enrolment = await db.Enrolments.Include(e => e.Category).ThenInclude(c => c!.Event).SingleOrDefaultAsync(e => e.EnrolmentID == enrolmentId);
        if (enrolment is null) return NotFound();
        if (enrolment.Category!.Event!.OrganiserID != ownerId) return Forbid();
        if (await db.Results.AnyAsync(r => r.EnrolmentID == enrolmentId)) return Conflict(new { message = "A result already exists for this enrolment." });
        var result = new Result { EnrolmentID = enrolmentId, FinishTime = request.FinishTime, Position = request.Position, Status = request.Status };
        db.Results.Add(result); await db.SaveChangesAsync(); return Created($"/api/events/{enrolment.Category.EventID}/results", result);
    }

    /// <summary>Updates a result only if the logged-in Organiser owns the associated event.</summary>
    [HttpPut("results/{id:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Update(int id, ResultRequest request)
    {
        var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await db.Results.Include(r => r.Enrolment).ThenInclude(e => e!.Category).ThenInclude(c => c!.Event).SingleOrDefaultAsync(r => r.ResultID == id);
        if (result is null) return NotFound();
        if (result.Enrolment!.Category!.Event!.OrganiserID != ownerId) return Forbid();
        result.FinishTime = request.FinishTime; result.Position = request.Position; result.Status = request.Status;
        await db.SaveChangesAsync(); return Ok(result);
    }

    /// <summary>Returns the authenticated Participant's own result history.</summary>
    [HttpGet("results/my")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> Mine()
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await db.Results.AsNoTracking().Where(r => r.Enrolment!.ParticipantID == id).Include(r => r.Enrolment)!.ThenInclude(e => e!.Category)!.ThenInclude(c => c!.Event).ToListAsync());
    }

    /// <summary>Public leaderboard for an event.</summary>
    [HttpGet("events/{eventId:int}/results")]
    [AllowAnonymous]
    public async Task<IActionResult> ForEvent(int eventId)
    {
        if (!await db.Events.AnyAsync(e => e.EventID == eventId)) return NotFound();
        return Ok(await db.Results.AsNoTracking().Where(r => r.Enrolment!.Category!.EventID == eventId)
            .Include(r => r.Enrolment)!.ThenInclude(e => e!.Participant)
            .Include(r => r.Enrolment)!.ThenInclude(e => e!.Category)
            .OrderBy(r => r.Position).ToListAsync());
    }
}
