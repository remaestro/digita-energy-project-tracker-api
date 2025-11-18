using AutoMapper;
using DigitaEnergy.ProjectTracker.Application.DTOs.Tasks;
using DigitaEnergy.ProjectTracker.Application.DTOs.Milestones;
using DigitaEnergy.ProjectTracker.Application.DTOs.Risks;
using DigitaEnergy.ProjectTracker.Domain.Entities;

namespace DigitaEnergy.ProjectTracker.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Task Mappings
        CreateMap<Domain.Entities.Task, TaskDto>().ReverseMap();

        // Milestone Mappings
        CreateMap<Milestone, MilestoneDto>().ReverseMap();
        CreateMap<CreateMilestoneDto, Milestone>();
        CreateMap<UpdateMilestoneDto, Milestone>();

        // Risk Mappings
        CreateMap<Risk, RiskDto>().ReverseMap();
        CreateMap<CreateRiskDto, Risk>();
        CreateMap<UpdateRiskDto, Risk>();
    }
}
