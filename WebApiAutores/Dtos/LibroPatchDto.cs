using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Dtos
{
    public class LibroPatchDto
    {
        [PrimeraLetraMayuscula]
        [StringLength(250)]
        public string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}
