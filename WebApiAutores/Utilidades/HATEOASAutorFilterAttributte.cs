using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using WebApiAutores.Dtos;
using WebApiAutores.Servicios;

namespace WebApiAutores.Utilidades
{
    public class HATEOASAutorFilterAttributte : HATEOASFiltroAttributte
    {
        private readonly GeneradorEnlaces generadorEnlaces;

        public HATEOASAutorFilterAttributte(GeneradorEnlaces generadorEnlaces)
        {
            this.generadorEnlaces = generadorEnlaces;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var debeIncluir = DebeIncluirHATEOAS(context);

            if (!debeIncluir)
            {
                await next();
                return;
            }

            var resultado = context.Result as ObjectResult;
            var modelo = resultado.Value as AutorGetDto ?? throw new ArgumentNullException("Se esperaba un instancia de AutorDto");
            await generadorEnlaces.generarEnlaces(modelo);
            await next();
        }
    }
}
