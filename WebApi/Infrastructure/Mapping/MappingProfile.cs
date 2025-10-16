using AutoMapper;
using WebApi.Domain.Dtos;
using WebApi.Domain.Entities;
using Host = WebApi.Domain.Entities.Host;

namespace WebApi.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Property, Property_DTO>().ReverseMap();

            CreateMap<Host, Host_DTO>().ReverseMap();
        }
    }
}