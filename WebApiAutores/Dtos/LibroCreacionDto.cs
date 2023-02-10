using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Dtos
{
    public class LibroCreacionDto
    {
        [PrimeraLetraMayuscula]
        [StringLength(250)]
        public string Titulo { get; set; }

        public List<int> AutoresIds { get; set; }
    }
}
