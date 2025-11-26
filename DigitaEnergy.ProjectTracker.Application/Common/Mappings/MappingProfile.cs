using AutoMapper;
using DigitaEnergy.ProjectTracker.Application.DTOs.Tasks;
using DigitaEnergy.ProjectTracker.Application.DTOs.Milestones;
using DigitaEnergy.ProjectTracker.Application.DTOs.Risks;
using DigitaEnergy.ProjectTracker.Application.DTOs.Auth;
using DigitaEnergy.ProjectTracker.Domain.Entities;
using DigitaEnergy.ProjectTracker.Domain.Enums;

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

        // User/Auth Mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.AssignedWorkstreams, opt => opt.MapFrom(src => 
                ConvertWorkstreamsToDisplayValues(src.AssignedWorkstreams)));
        
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.AssignedWorkstreams, opt => opt.MapFrom(src => 
                src.AssignedWorkstreams != null && src.AssignedWorkstreams.Any() 
                    ? string.Join(",", src.AssignedWorkstreams) 
                    : null));
        
        CreateMap<RegisterRequestDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedWorkstreams, opt => opt.MapFrom(src => 
                src.AssignedWorkstreams != null && src.AssignedWorkstreams.Any() 
                    ? string.Join(",", src.AssignedWorkstreams) 
                    : null))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        
        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedWorkstreams, opt => opt.MapFrom(src => 
                src.AssignedWorkstreams != null && src.AssignedWorkstreams.Any() 
                    ? string.Join(",", src.AssignedWorkstreams) 
                    : null))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ResetToken, opt => opt.Ignore())
            .ForMember(dest => dest.ResetTokenExpiry, opt => opt.Ignore());
    }

    // Méthode helper pour convertir les workstreams enum en valeurs de base de données
    private static List<string> ConvertWorkstreamsToDisplayValues(string? assignedWorkstreams)
    {
        if (string.IsNullOrWhiteSpace(assignedWorkstreams))
        {
            return new List<string>();
        }

        var workstreamNames = assignedWorkstreams.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var result = new List<string>();

        foreach (var name in workstreamNames)
        {
            if (Enum.TryParse<Workstream>(name.Trim(), out var workstream))
            {
                result.Add(workstream.ToDbValue());
            }
            else
            {
                result.Add(name.Trim());
            }
        }

        return result;
    }
}
