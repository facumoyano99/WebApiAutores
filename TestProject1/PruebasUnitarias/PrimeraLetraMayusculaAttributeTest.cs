using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Tests.PruebasUnitarias
{
    [TestClass]
    public class PrimeraLetraMayusculaAttributeTest
    {
        [TestMethod]
        public void PrimeraLetraMiniscula_DevuelveError()
        {
            //Preparacion
            var primeraLetraMayus = new PrimeraLetraMayusculaAttribute();
            var valor = "felipe";
            var valContext = new ValidationContext(new { Nombre = valor});
            //Ejecucion
            var resultado = primeraLetraMayus.GetValidationResult(valor, valContext);

            //Verificacion
            Assert.AreEqual("La primera letra debe ser mayúscula", resultado.ErrorMessage);
        }

        [TestMethod]
        public void ValorNulo_NoDevuelveError()
        {
            //Preparacion
            var primeraLetraMayus = new PrimeraLetraMayusculaAttribute();
            string valor = null;
            var valContext = new ValidationContext(new { Nombre = valor });
            //Ejecucion
            var resultado = primeraLetraMayus.GetValidationResult(valor, valContext);

            //Verificacion
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void ValorConPrimeraLetraMayus_NoDevuelveError()
        {
            //Preparacion
            var primeraLetraMayus = new PrimeraLetraMayusculaAttribute();
            string valor = "Felipe";
            var valContext = new ValidationContext(new { Nombre = valor });
            //Ejecucion
            var resultado = primeraLetraMayus.GetValidationResult(valor, valContext);

            //Verificacion
            Assert.IsNull(resultado);
        }
    }
}