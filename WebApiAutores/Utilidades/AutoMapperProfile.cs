using AutoMapper;
using WebApiAutores.Dtos;
using WebApiAutores.Entidades;

namespace WebApiAutores.Utilidades
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AutorCreacionDto, Autor>();
        }
    }
}
