using DigitaEnergy.ProjectTracker.Application.DTOs.Risks;
using DigitaEnergy.ProjectTracker.Application.Interfaces;
using DigitaEnergy.ProjectTracker.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitaEnergy.ProjectTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RisksController : ControllerBase
{
    private readonly IRiskService _riskService;
    private readonly IWorkstreamAuthorizationService _authorizationService;

    public RisksController(IRiskService riskService, IWorkstreamAuthorizationService authorizationService)
    {
        _riskService = riskService;
        _authorizationService = authorizationService;
    }
    
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found"));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RiskDto>>> GetRisks()
    {
        var userId = GetCurrentUserId();
        var accessibleWorkstreams = await _authorizationService.GetFilteredWorkstreamsAsync(userId);
        var workstreamDbValues = accessibleWorkstreams.Select(w => w.ToDbValue()).ToList();
        
        var allRisks = await _riskService.GetAllRisksAsync();
        
        // Filtrer les risques selon les workstreams accessibles
        var filteredRisks = allRisks.Where(r => 
            string.IsNullOrWhiteSpace(r.Workstream) || 
            workstreamDbValues.Contains(r.Workstream)
        ).ToList();
        
        return Ok(new { Data = filteredRisks, TotalCount = filteredRisks.Count });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RiskDto>> GetRisk(int id)
    {
        var risk = await _riskService.GetRiskByIdAsync(id);
        if (risk == null)
        {
            return NotFound();
        }
        
        // Vérifier l'accès au workstream
        var userId = GetCurrentUserId();
        var accessibleWorkstreams = await _authorizationService.GetFilteredWorkstreamsAsync(userId);
        var workstreamDbValues = accessibleWorkstreams.Select(w => w.ToDbValue()).ToList();
        
        if (!string.IsNullOrWhiteSpace(risk.Workstream) && !workstreamDbValues.Contains(risk.Workstream))
        {
            return Forbid();
        }
        
        return Ok(risk);
    }

    [HttpPost]
    public async Task<ActionResult<RiskDto>> CreateRisk(CreateRiskDto riskDto)
    {
        var userId = GetCurrentUserId();
        
        // Vérifier les permissions de modification
        if (!string.IsNullOrWhiteSpace(riskDto.Workstream))
        {
            var workstream = WorkstreamExtensions.FromDbValue(riskDto.Workstream);
            if (workstream.HasValue)
            {
                var canModify = await _authorizationService.CanModifyInWorkstreamAsync(userId, workstream.Value);
                if (!canModify)
                {
                    return Forbid();
                }
            }
        }
        
        var createdRisk = await _riskService.CreateRiskAsync(riskDto);
        return CreatedAtAction(nameof(GetRisk), new { id = createdRisk.Id }, createdRisk);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRisk(int id, UpdateRiskDto riskDto)
    {
        if (id != riskDto.Id)
        {
            return BadRequest();
        }
        
        var userId = GetCurrentUserId();
        
        // Vérifier les permissions de modification
        if (!string.IsNullOrWhiteSpace(riskDto.Workstream))
        {
            var workstream = WorkstreamExtensions.FromDbValue(riskDto.Workstream);
            if (workstream.HasValue)
            {
                var canModify = await _authorizationService.CanModifyInWorkstreamAsync(userId, workstream.Value);
                if (!canModify)
                {
                    return Forbid();
                }
            }
        }

        await _riskService.UpdateRiskAsync(id, riskDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRisk(int id)
    {
        var userId = GetCurrentUserId();
        var risk = await _riskService.GetRiskByIdAsync(id);
        
        if (risk == null)
        {
            return NotFound();
        }
        
        // Vérifier les permissions de modification
        if (!string.IsNullOrWhiteSpace(risk.Workstream))
        {
            var workstream = WorkstreamExtensions.FromDbValue(risk.Workstream);
            if (workstream.HasValue)
            {
                var canModify = await _authorizationService.CanModifyInWorkstreamAsync(userId, workstream.Value);
                if (!canModify)
                {
                    return Forbid();
                }
            }
        }
        
        await _riskService.DeleteRiskAsync(id);
        return NoContent();
    }
}
