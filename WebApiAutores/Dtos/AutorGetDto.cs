using System.Collections.Generic;

namespace WebApiAutores.Dtos
{
    public class AutorGetDto: Recurso
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
