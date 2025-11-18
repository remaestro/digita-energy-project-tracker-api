using DigitaEnergy.ProjectTracker.Application.DTOs.Milestones;
using DigitaEnergy.ProjectTracker.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitaEnergy.ProjectTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MilestonesController : ControllerBase
{
    private readonly IMilestoneService _milestoneService;

    public MilestonesController(IMilestoneService milestoneService)
    {
        _milestoneService = milestoneService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MilestoneDto>>> GetMilestones()
    {
        var milestones = await _milestoneService.GetAllMilestonesAsync();
        return Ok(new { Data = milestones, TotalCount = milestones.Count() });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MilestoneDto>> GetMilestone(int id)
    {
        var milestone = await _milestoneService.GetMilestoneByIdAsync(id);
        if (milestone == null)
        {
            return NotFound();
        }
        return Ok(milestone);
    }

    [HttpPost]
    public async Task<ActionResult<MilestoneDto>> CreateMilestone(CreateMilestoneDto milestoneDto)
    {
        var createdMilestone = await _milestoneService.CreateMilestoneAsync(milestoneDto);
        return CreatedAtAction(nameof(GetMilestone), new { id = createdMilestone.Id }, createdMilestone);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMilestone(int id, UpdateMilestoneDto milestoneDto)
    {
        if (id != milestoneDto.Id)
        {
            return BadRequest();
        }

        await _milestoneService.UpdateMilestoneAsync(id, milestoneDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMilestone(int id)
    {
        await _milestoneService.DeleteMilestoneAsync(id);
        return NoContent();
    }
}
