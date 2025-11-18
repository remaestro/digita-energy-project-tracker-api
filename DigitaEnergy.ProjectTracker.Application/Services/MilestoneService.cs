using DigitaEnergy.ProjectTracker.Application.DTOs.Milestones;
using DigitaEnergy.ProjectTracker.Application.Interfaces;
using DigitaEnergy.ProjectTracker.Domain.Entities;
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

        public async Task<MilestoneDto?> GetMilestoneByIdAsync(int id)
        {
            var milestone = await _milestoneRepository.GetByIdAsync(id);
            if (milestone == null) return null;

            return new MilestoneDto
            {
                Id = milestone.Id,
                Title = milestone.Title,
                Description = milestone.Description,
                DueDate = milestone.DueDate,
                Status = milestone.Status
            };
        }

        public async Task<IEnumerable<MilestoneDto>> GetAllMilestonesAsync()
        {
            var milestones = await _milestoneRepository.GetAllAsync();
            return milestones.Select(milestone => new MilestoneDto
            {
                Id = milestone.Id,
                Title = milestone.Title,
                Description = milestone.Description,
                DueDate = milestone.DueDate,
                Status = milestone.Status
            });
        }

        public async Task<MilestoneDto> CreateMilestoneAsync(CreateMilestoneDto milestoneDto)
        {
            var milestone = new Milestone
            {
                Title = milestoneDto.Title ?? string.Empty,
                Description = milestoneDto.Description ?? string.Empty,
                DueDate = milestoneDto.DueDate,
                Status = milestoneDto.Status ?? string.Empty,
                LinkedTaskIds = milestoneDto.LinkedTaskIds ?? new List<int>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _milestoneRepository.AddAsync(milestone);

            return new MilestoneDto
            {
                Id = milestone.Id,
                Title = milestone.Title,
                Description = milestone.Description,
                DueDate = milestone.DueDate,
                Status = milestone.Status
            };
        }

        public async Task UpdateMilestoneAsync(int id, UpdateMilestoneDto milestoneDto)
        {
            var milestone = await _milestoneRepository.GetByIdAsync(id);
            if (milestone == null) return;

            milestone.Title = milestoneDto.Title ?? milestone.Title;
            milestone.Description = milestoneDto.Description ?? milestone.Description;
            milestone.DueDate = milestoneDto.DueDate;
            milestone.Status = milestoneDto.Status ?? milestone.Status;
            milestone.UpdatedAt = DateTime.UtcNow;

            await _milestoneRepository.UpdateAsync(milestone);
        }

        public async Task DeleteMilestoneAsync(int id)
        {
            await _milestoneRepository.DeleteAsync(id);
        }
    }
}