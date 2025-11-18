using DigitaEnergy.ProjectTracker.Application.DTOs.Risks;
using DigitaEnergy.ProjectTracker.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitaEnergy.ProjectTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RisksController : ControllerBase
{
    private readonly IRiskService _riskService;

    public RisksController(IRiskService riskService)
    {
        _riskService = riskService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RiskDto>>> GetRisks()
    {
        var risks = await _riskService.GetAllRisksAsync();
        return Ok(new { Data = risks, TotalCount = risks.Count() });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RiskDto>> GetRisk(int id)
    {
        var risk = await _riskService.GetRiskByIdAsync(id);
        if (risk == null)
        {
            return NotFound();
        }
        return Ok(risk);
    }

    [HttpPost]
    public async Task<ActionResult<RiskDto>> CreateRisk(CreateRiskDto riskDto)
    {
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

        await _riskService.UpdateRiskAsync(id, riskDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRisk(int id)
    {
        await _riskService.DeleteRiskAsync(id);
        return NoContent();
    }
}
