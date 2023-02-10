using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApiAutores.Entidades;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Dtos
{
    public class LibroGetDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
    }
}
