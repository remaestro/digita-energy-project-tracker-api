using DigitaEnergy.ProjectTracker.Application.DTOs.Milestones;
using DigitaEnergy.ProjectTracker.Application.Interfaces;
using DigitaEnergy.ProjectTracker.Domain.Entities;
using DigitaEnergy.ProjectTracker.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Task = System.Threading.Tasks.Task;

namespace DigitaEnergy.ProjectTracker.Application.Services
{
    public class MilestoneService : IMilestoneService
    {
        private readonly IMilestoneRepository _milestoneRepository;

        public MilestoneService(IMilestoneRepository milestoneRepository)
        {
            _milestoneRepository = milestoneRepository;
        }

        public async Task<MilestoneDto> GetMilestoneByIdAsync(int id)
        {
            var milestone = await _milestoneRepository.GetByIdAsync(id);
            if (milestone == null) 
                throw new KeyNotFoundException($"Milestone with id {id} not found");

            return new MilestoneDto
            {
                Id = milestone.Id,
                Code = milestone.Code,
                Title = milestone.Title,
                Workstream = milestone.Workstream,
                DatePlanned = milestone.DatePlanned,
                DateActual = milestone.DateActual,
                Status = milestone.Status,
                Comments = milestone.Comments,
                LinkedTaskIds = milestone.LinkedTaskIds
            };
        }

        public async Task<IEnumerable<MilestoneDto>> GetAllMilestonesAsync()
        {
            var milestones = await _milestoneRepository.GetAllAsync();
            return milestones.Select(milestone => new MilestoneDto
            {
                Id = milestone.Id,
                Code = milestone.Code,
                Title = milestone.Title,
                Workstream = milestone.Workstream,
                DatePlanned = milestone.DatePlanned,
                DateActual = milestone.DateActual,
                Status = milestone.Status,
                Comments = milestone.Comments,
                LinkedTaskIds = milestone.LinkedTaskIds
            });
        }

        public async Task<MilestoneDto> CreateMilestoneAsync(CreateMilestoneDto milestoneDto)
        {
            var milestone = new Milestone
            {
                Code = milestoneDto.Code,
                Title = milestoneDto.Title ?? string.Empty,
                Workstream = milestoneDto.Workstream,
                DatePlanned = milestoneDto.DatePlanned,
                DateActual = milestoneDto.DateActual,
                Status = milestoneDto.Status ?? string.Empty,
                Comments = milestoneDto.Comments,
                LinkedTaskIds = milestoneDto.LinkedTaskIds ?? new List<int>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _milestoneRepository.AddAsync(milestone);

            return new MilestoneDto
            {
                Id = milestone.Id,
                Code = milestone.Code,
                Title = milestone.Title,
                Workstream = milestone.Workstream,
                DatePlanned = milestone.DatePlanned,
                DateActual = milestone.DateActual,
                Status = milestone.Status,
                Comments = milestone.Comments,
                LinkedTaskIds = milestone.LinkedTaskIds
            };
        }

        public async Task UpdateMilestoneAsync(int id, UpdateMilestoneDto milestoneDto)
        {
            var milestone = await _milestoneRepository.GetByIdAsync(id);
            if (milestone == null) return;

            milestone.Code = milestoneDto.Code ?? milestone.Code;
            milestone.Title = milestoneDto.Title ?? milestone.Title;
            milestone.Workstream = milestoneDto.Workstream ?? milestone.Workstream;
            milestone.DatePlanned = milestoneDto.DatePlanned;
            milestone.DateActual = milestoneDto.DateActual;
            milestone.Status = milestoneDto.Status ?? milestone.Status;
            milestone.Comments = milestoneDto.Comments ?? milestone.Comments;
            milestone.LinkedTaskIds = milestoneDto.LinkedTaskIds ?? milestone.LinkedTaskIds;
            milestone.UpdatedAt = DateTime.UtcNow;

            await _milestoneRepository.UpdateAsync(milestone);
        }

        public async Task DeleteMilestoneAsync(int id)
        {
            await _milestoneRepository.DeleteAsync(id);
        }
    }
}