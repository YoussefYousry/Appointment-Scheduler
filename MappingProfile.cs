using AutoMapper;
using BusinessLayer_AppointmentScheduler.Dto;
using DataLayer_AppointmentScheduler.Models;

namespace AppointmentScheduler
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            //POST -> DTO FIRST
            //GET -> DbSet FIRST
            CreateMap<Employee, SingleEmployeeDTO>()
                .ForMember(e => e.FullName,
                opts => opts.MapFrom(x => String.Join(' ', x.FirstName, x.LastName)));
            CreateMap<Employee, AllEmployeesDTO>()
    .ForMember(e => e.FullName,
    opts => opts.MapFrom(x => String.Join(' ', x.FirstName, x.LastName)));
            //CreateMap<UserForRegisterationDTO, User>();
            CreateMap<UserForRegisterationDTO, Employee>();
            CreateMap<UserForRegisterationDTO, Manager>();
            CreateMap<UserForRegisterationDTO, Admin>();
            CreateMap<UserForLoginDTO, User>();

            CreateMap<SkillForCreationDTO, Skill>();
            CreateMap<Skill,SkillsDTO>();
            CreateMap<DepartmentForCreationDTO, Department>();
            CreateMap<Manager, SingleManagerDTO>()
                .ForMember(e => e.FullName,
                opts => opts.MapFrom(x => String.Join(' ', x.FirstName, x.LastName)));
            CreateMap<Manager, AllManagersDTO>()
                .ForMember(e => e.FullName,
                opts => opts.MapFrom(x => String.Join(' ', x.FirstName, x.LastName)));

            CreateMap<EmployeeForUpdateDTO, Employee>();
            CreateMap<ManagerForUpdateDTO, Manager>();
            CreateMap<PatchDepartmentDTO, Employee>();
        }
    }
}
