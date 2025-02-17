using AutoMapper;
using Wine.Application.Dtos;

namespace Wine.Application.Mappers;


public class WineProfile : Profile
{
    public WineProfile()
    {
        CreateMap<Domain.Entities.Wine, WineDto>().ReverseMap();
    }
}