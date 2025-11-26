using DigitaEnergy.ProjectTracker.Application.DTOs.Milestones;
using DigitaEnergy.ProjectTracker.Application.Interfaces;
using DigitaEnergy.ProjectTracker.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitaEnergy.ProjectTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MilestonesController : ControllerBase
{
    private readonly IMilestoneService _milestoneService;
    private readonly IWorkstreamAuthorizationService _authorizationService;

    public MilestonesController(IMilestoneService milestoneService, IWorkstreamAuthorizationService authorizationService)
    {
        _milestoneService = milestoneService;
        _authorizationService = authorizationService;
    }
    
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found"));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MilestoneDto>>> GetMilestones([FromQuery] string? viewMode = null)
    {
        var userId = GetCurrentUserId();
        
        // Si viewMode=gantt, utiliser GetAccessibleWorkstreamsAsync (vue complète)
        // Sinon, utiliser GetFilteredWorkstreamsAsync (vue filtrée par workstreams assignés)
        var accessibleWorkstreams = viewMode?.ToLower() == "gantt"
            ? await _authorizationService.GetAccessibleWorkstreamsAsync(userId)
            : await _authorizationService.GetFilteredWorkstreamsAsync(userId);
        
        var allMilestones = await _milestoneService.GetAllMilestonesAsync();
        
        // Filtrer les milestones selon les workstreams accessibles
        // Utiliser FromDbValue pour normaliser la comparaison et gérer les variantes
        var filteredMilestones = allMilestones.Where(m => 
        {
            if (string.IsNullOrWhiteSpace(m.Workstream)) return true;
            
            // Essayer de convertir le workstream du milestone en enum
            var milestoneWorkstream = WorkstreamExtensions.FromDbValue(m.Workstream);
            if (!milestoneWorkstream.HasValue) return false;
            
            // Vérifier si ce workstream est dans la liste accessible
            return accessibleWorkstreams.Contains(milestoneWorkstream.Value);
        }).ToList();
        
        return Ok(new { Data = filteredMilestones, TotalCount = filteredMilestones.Count });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MilestoneDto>> GetMilestone(int id)
    {
        var milestone = await _milestoneService.GetMilestoneByIdAsync(id);
        if (milestone == null)
        {
            return NotFound();
        }
        
        // Vérifier l'accès au workstream
        var userId = GetCurrentUserId();
        var accessibleWorkstreams = await _authorizationService.GetFilteredWorkstreamsAsync(userId);
        var workstreamDbValues = accessibleWorkstreams.Select(w => w.ToDbValue()).ToList();
        
        if (!string.IsNullOrWhiteSpace(milestone.Workstream) && !workstreamDbValues.Contains(milestone.Workstream))
        {
            return Forbid();
        }
        
        return Ok(milestone);
    }

    [HttpPost]
    public async Task<ActionResult<MilestoneDto>> CreateMilestone(CreateMilestoneDto milestoneDto)
    {
        var userId = GetCurrentUserId();
        
        // Vérifier les permissions de modification
        if (!string.IsNullOrWhiteSpace(milestoneDto.Workstream))
        {
            var workstream = WorkstreamExtensions.FromDbValue(milestoneDto.Workstream);
            if (workstream.HasValue)
            {
                var canModify = await _authorizationService.CanModifyInWorkstreamAsync(userId, workstream.Value);
                if (!canModify)
                {
                    return Forbid();
                }
            }
        }
        
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
        
        var userId = GetCurrentUserId();
        
        // Vérifier les permissions de modification
        if (!string.IsNullOrWhiteSpace(milestoneDto.Workstream))
        {
            var workstream = WorkstreamExtensions.FromDbValue(milestoneDto.Workstream);
            if (workstream.HasValue)
            {
                var canModify = await _authorizationService.CanModifyInWorkstreamAsync(userId, workstream.Value);
                if (!canModify)
                {
                    return Forbid();
                }
            }
        }

        await _milestoneService.UpdateMilestoneAsync(id, milestoneDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMilestone(int id)
    {
        var userId = GetCurrentUserId();
        var milestone = await _milestoneService.GetMilestoneByIdAsync(id);
        
        if (milestone == null)
        {
            return NotFound();
        }
        
        // Vérifier les permissions de modification
        if (!string.IsNullOrWhiteSpace(milestone.Workstream))
        {
            var workstream = WorkstreamExtensions.FromDbValue(milestone.Workstream);
            if (workstream.HasValue)
            {
                var canModify = await _authorizationService.CanModifyInWorkstreamAsync(userId, workstream.Value);
                if (!canModify)
                {
                    return Forbid();
                }
            }
        }
        
        await _milestoneService.DeleteMilestoneAsync(id);
        return NoContent();
    }
}
