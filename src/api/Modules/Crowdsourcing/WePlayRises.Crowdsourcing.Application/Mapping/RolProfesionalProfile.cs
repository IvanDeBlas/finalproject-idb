using AutoMapper;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Mapping;

public class RolProfesionalProfile : Profile
{
    public RolProfesionalProfile()
    {
        CreateMap<MaestraRolProfesional, RolProfesionalDto>();

        CreateMap<MaestraRolProfesional, RolProfesionalConCategoriaDto>()
            .ForMember(dest => dest.CategoriaRol,
                opt => opt.MapFrom(src => src.CategoriaRol));
    }
}
