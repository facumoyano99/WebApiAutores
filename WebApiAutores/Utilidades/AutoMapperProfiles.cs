using AutoMapper;
using System.Collections.Generic;
using WebApiAutores.Dtos;
using WebApiAutores.Entidades;

namespace WebApiAutores.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AutorCreacionDto, Autor>();
            CreateMap<Autor, AutorGetDto>();
            CreateMap<Autor, AutorDtoConLibros>().ForMember(autorDto=> autorDto.Libros, opciones => opciones.MapFrom(MapAutorDtoLibros));


            CreateMap<LibroCreacionDto, Libro>()
                .ForMember(libro => libro.AutoresLibros, opciones => opciones.MapFrom(MapAutoresLibros));
            CreateMap<Libro, LibroGetDto>();

            CreateMap<Libro, LibroDtoConAutores>()
                .ForMember(ld=>ld.Autores, opciones => opciones.MapFrom(MapLibroDtoAutores));
            CreateMap<LibroPatchDto, Libro>().ReverseMap();


            CreateMap<ComentarioCreacionDto, Comentario>();
            CreateMap<Comentario, ComentarioGetDto>();
        }
        private List<AutorLibro> MapAutoresLibros(LibroCreacionDto libroDto, Libro libro)
        {
            var resultado = new List<AutorLibro>();

            if (libroDto.AutoresIds == null) { return resultado; }

            foreach (var autorId in libroDto.AutoresIds)
            {
                resultado.Add(new AutorLibro() { AutorId = autorId });
            }

            return resultado;
        }

        private List<AutorGetDto> MapLibroDtoAutores(Libro libro, LibroGetDto libroDto)
        {
            var resultado = new List<AutorGetDto>();

            if (libro.AutoresLibros == null) { return resultado; }

            foreach (var autorLibro in libro.AutoresLibros)
            {
                resultado.Add(new AutorGetDto() { Id = autorLibro.AutorId, Nombre = autorLibro.Autor.Nombre });
            }

            return resultado;
        }

        private List<LibroGetDto> MapAutorDtoLibros (Autor autor, AutorGetDto autorDto)
        {
            var resultado = new List<LibroGetDto>();
            if (autor.AutoresLibros == null) { return resultado; }

            foreach (var autorLibro in autor.AutoresLibros)
            {
                resultado.Add(new LibroGetDto()
                {
                    Id = autorLibro.LibroId,
                    Titulo = autorLibro.Libro.Titulo
                });
            }
            return resultado;
        }
    }
}
