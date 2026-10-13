using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class EnrolmentsController(RaceDayDbContext db) : ControllerBase
{
    /// <summary>Enrols the logged-in Participant in a category and assigns a bib number.</summary>
    [HttpPost("categories/{categoryId:int}/enrolments")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> Enrol(int categoryId)
    {
        var participantId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var category = await db.Categories.Include(c => c.Enrolments).SingleOrDefaultAsync(c => c.CategoryID == categoryId);
        if (category is null) return NotFound(new { message = "Category not found." });
        if (category.Enrolments.Any(e => e.ParticipantID == participantId)) return Conflict(new { message = "You are already enrolled in this category." });
        if (category.Enrolments.Count(e => e.Status == "Confirmed") >= category.MaxParticipants) return Conflict(new { message = "This category is full." });
        var enrolment = new Enrolment { ParticipantID = participantId, CategoryID = categoryId, EnrolmentDate = DateTime.Now,
            BibNumber = $"{Random.Shared.Next(1, 99999):D5}", Status = "Confirmed" };
        db.Enrolments.Add(enrolment);
        try { await db.SaveChangesAsync(); } catch (DbUpdateException) { return Conflict(new { message = "Could not complete enrolment; check for a duplicate." }); }
        return Created($"/api/enrolments/my", enrolment);
    }

    /// <summary>Lists the authenticated Participant's enrolments.</summary>
    [HttpGet("enrolments/my")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> Mine()
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await db.Enrolments.AsNoTracking().Where(e => e.ParticipantID == id).Include(e => e.Category)!.ThenInclude(c => c!.Event).ToListAsync());
    }

    /// <summary>Lists enrolments for an event owned by the authenticated Organiser.</summary>
    [HttpGet("events/{eventId:int}/enrolments")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> ForEvent(int eventId)
    {
        var ownerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var item = await db.Events.SingleOrDefaultAsync(e => e.EventID == eventId);
        if (item is null) return NotFound();
        if (item.OrganiserID != ownerId) return Forbid();
        return Ok(await db.Enrolments.AsNoTracking().Where(e => e.Category!.EventID == eventId).Include(e => e.Participant).Include(e => e.Category).ToListAsync());
    }

    /// <summary>Cancels the authenticated Participant's own enrolment.</summary>
    [HttpDelete("enrolments/{id:int}")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> Cancel(int id)
    {
        var participantId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var enrolment = await db.Enrolments.SingleOrDefaultAsync(e => e.EnrolmentID == id);
        if (enrolment is null) return NotFound();
        if (enrolment.ParticipantID != participantId) return Forbid();
        enrolment.Status = "Cancelled"; await db.SaveChangesAsync(); return NoContent();
    }
}
