using System.Collections.Generic;

namespace WebApiAutores.Dtos
{
    public class LibroDtoConAutores : LibroGetDto
    {
        public List<AutorGetDto> Autores { get; set; }
    }
}
