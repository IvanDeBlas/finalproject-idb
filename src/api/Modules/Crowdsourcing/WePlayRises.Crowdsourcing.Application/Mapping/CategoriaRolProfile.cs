using AutoMapper;
using WePlayRises.Crowdsourcing.Application.Dtos;
using WePlayRises.Crowdsourcing.Domain.Model;

namespace WePlayRises.Crowdsourcing.Application.Mapping;

public class CategoriaRolProfile : Profile
{
    public CategoriaRolProfile()
    {
        CreateMap<MaestraCategoriaRol, CategoriaRolDto>();
    }
}
