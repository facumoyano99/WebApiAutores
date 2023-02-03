using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Dtos
{
    public class AutorCreacionDto
    {

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no debe tener mas de {1} carácteres")] //{0} se refiere al nombre del atributo que esta validando. {1} se refiere al maxlengt
        [PrimeraLetraMayuscula] 
        public string Nombre { get; set; }
    }
}
