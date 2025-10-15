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

            CreateMap<RegisterEmployerViewModel, RegisterEmployerDto>();
            CreateMap<RegisterJobSeekerViewModel, RegisterJobSeekerDto>();

            CreateMap<RegisterEmployerDto, Employer>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<RegisterJobSeekerDto, JobSeeker>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<User, UserProfileDto>()
                .ForMember(dest => dest.Role, opt => opt.Ignore());
        }
    }
}
