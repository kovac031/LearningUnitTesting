using AutoMapper;
using Model;
using WebAPI;

namespace Common
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Student, StudentDTO>();

            CreateMap<StudentDTO, Student>() // nemoj mapirati novi Id nikada, ovdje ako dobijes NULL kod mapiranja, mapira staru vrijednost
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FirstName, opt => opt.Condition(src => !string.IsNullOrEmpty(src.FirstName)))
                .ForMember(dest => dest.LastName, opt => opt.Condition(src => !string.IsNullOrEmpty(src.LastName)))
                .ForMember(dest => dest.DateOfBirth, opt => opt.Condition(src => src.DateOfBirth != default(DateTime)))
                .ForMember(dest => dest.EmailAddress, opt => opt.Condition(src => !string.IsNullOrEmpty(src.EmailAddress)))
                .ForMember(dest => dest.RegisteredOn, opt => opt.Condition(src => src.RegisteredOn != default(DateTime)));
        }
    }
}