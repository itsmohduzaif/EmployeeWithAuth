using AutoMapper;
using HRManagement.DTOs.DraftDTOs;
using HRManagement.DTOs.EmployeeDTOs;
using HRManagement.Models;

namespace HRManagement.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeCreateDTO>();
            CreateMap<EmployeeCreateDTO, Employee>();
            CreateMap<Employee, SignUpAsAnEmployeeDTO>();
            CreateMap<SignUpAsAnEmployeeDTO, Employee>();
            CreateMap<Employee, EmployeeUpdateDTO>();
            CreateMap<EmployeeUpdateDTO, Employee>();
            //CreateMap<Employee, EmployeeProfileDTO>().ForMember(dest => dest.ProfilePictureUrl, opt => opt.Ignore());
            CreateMap<Employee, EmployeeProfileDTO>()
                .ForMember(dest => dest.ProfilePictureUrl, opt => opt.Ignore());  // set manually after mapping
                //.ForMember(dest => dest.EmployeeFullName,
                //            opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()));


            CreateMap<EmployeeProfileUpdateDTO, Employee>();
            CreateMap<Employee, EmployeeProfileUpdateDTO>();
            CreateMap<Employee, EmployeeCreateDraftDTO>();
            CreateMap<EmployeeCreateDraftDTO, Employee>();

        }
    }

}
