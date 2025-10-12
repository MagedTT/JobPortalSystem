using AutoMapper;
using job_portal_system.Models.Domain;
using job_portal_system.Models.DTOs;
using job_portal_system.Models.ViewModels;

namespace job_portal_system.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Job, JobDto>().ReverseMap();
            
            CreateMap<JobDto, JobViewModel>().ReverseMap();
        }
    }
}
