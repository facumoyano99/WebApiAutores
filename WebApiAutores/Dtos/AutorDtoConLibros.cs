using System.Collections.Generic;

namespace WebApiAutores.Dtos
{
    public class AutorDtoConLibros : AutorGetDto
    {
        public List<LibroGetDto> Libros { get; set; }
    }
}
