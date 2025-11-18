using DigitaEnergy.ProjectTracker.Application.DTOs.Risks;
using DigitaEnergy.ProjectTracker.Application.Interfaces;
using DigitaEnergy.ProjectTracker.Domain.Entities;
using DigitaEnergy.ProjectTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DigitaEnergy.ProjectTracker.Application.Services;

public class RiskService : IRiskService
{
    private readonly ProjectTrackerDbContext _context;

    public RiskService(ProjectTrackerDbContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task<IEnumerable<RiskDto>> GetAllRisksAsync()
    {
        var risks = await _context.Risks.ToListAsync();

        return risks.Select(r => new RiskDto
        {
            Id = r.Id,
            Title = r.Title,
            Workstream = r.Workstream,
            Probability = r.Probability,
            Impact = r.Impact,
            Criticality = r.Criticality,
            MitigationPlan = r.MitigationPlan,
            Owner = r.Owner,
            Status = r.Status
        });
    }

    public async System.Threading.Tasks.Task<RiskDto> GetRiskByIdAsync(int id)
    {
        var risk = await _context.Risks.FirstOrDefaultAsync(r => r.Id == id);

        if (risk == null)
            throw new KeyNotFoundException($"Risk with ID {id} not found.");

        return new RiskDto
        {
            Id = risk.Id,
            Title = risk.Title,
            Workstream = risk.Workstream,
            Probability = risk.Probability,
            Impact = risk.Impact,
            Criticality = risk.Criticality,
            MitigationPlan = risk.MitigationPlan,
            Owner = risk.Owner,
            Status = risk.Status
        };
    }

    public async System.Threading.Tasks.Task<RiskDto> CreateRiskAsync(CreateRiskDto riskDto)
    {
        var risk = new Risk
        {
            Title = riskDto.Title,
            Workstream = riskDto.Workstream,
            Probability = riskDto.Probability,
            Impact = riskDto.Impact,
            Criticality = riskDto.Probability * riskDto.Impact, // Auto-calculate criticality
            MitigationPlan = riskDto.MitigationPlan,
            Owner = riskDto.Owner,
            Status = riskDto.Status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Risks.Add(risk);
        await _context.SaveChangesAsync();

        return new RiskDto
        {
            Id = risk.Id,
            Title = risk.Title,
            Workstream = risk.Workstream,
            Probability = risk.Probability,
            Impact = risk.Impact,
            Criticality = risk.Criticality,
            MitigationPlan = risk.MitigationPlan,
            Owner = risk.Owner,
            Status = risk.Status
        };
    }

    public async System.Threading.Tasks.Task UpdateRiskAsync(int id, UpdateRiskDto riskDto)
    {
        var risk = await _context.Risks.FindAsync(id);
        if (risk == null)
            throw new KeyNotFoundException($"Risk with ID {id} not found.");

        risk.Title = riskDto.Title;
        risk.Workstream = riskDto.Workstream;
        risk.Probability = riskDto.Probability;
        risk.Impact = riskDto.Impact;
        risk.Criticality = riskDto.Probability * riskDto.Impact; // Recalculate criticality
        risk.MitigationPlan = riskDto.MitigationPlan;
        risk.Owner = riskDto.Owner;
        risk.Status = riskDto.Status;
        risk.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task DeleteRiskAsync(int id)
    {
        var risk = await _context.Risks.FindAsync(id);
        if (risk == null)
            throw new KeyNotFoundException($"Risk with ID {id} not found.");

        _context.Risks.Remove(risk);
        await _context.SaveChangesAsync();
    }
}
