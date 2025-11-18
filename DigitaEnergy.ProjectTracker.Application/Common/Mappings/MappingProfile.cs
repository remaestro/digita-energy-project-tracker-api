using AutoMapper;
using DigitaEnergy.ProjectTracker.Application.DTOs.Tasks;
using DigitaEnergy.ProjectTracker.Domain.Entities;

namespace DigitaEnergy.ProjectTracker.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Entities.Task, TaskDto>().ReverseMap();
    }
}
