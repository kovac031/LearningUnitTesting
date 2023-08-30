using AutoMapper;
using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Student, StudentDTO>();
            CreateMap<StudentDTO, Student>() // nemoj mapirati novi Id nikada, a ako dobijes NULL kod mapiranja, mapiraj staru vrijednost
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FirstName, opt => opt.Condition(src => !string.IsNullOrEmpty(src.FirstName)))
                .ForMember(dest => dest.LastName, opt => opt.Condition(src => !string.IsNullOrEmpty(src.LastName)))
                .ForMember(dest => dest.DateOfBirth, opt => opt.Condition(src => src.DateOfBirth != default(DateTime)))
                .ForMember(dest => dest.EmailAddress, opt => opt.Condition(src => !string.IsNullOrEmpty(src.EmailAddress)))
                .ForMember(dest => dest.RegisteredOn, opt => opt.Condition(src => src.RegisteredOn != default(DateTime)));

            CreateMap<StudentDTO, StudentView>().ReverseMap();
        }
    }
}
