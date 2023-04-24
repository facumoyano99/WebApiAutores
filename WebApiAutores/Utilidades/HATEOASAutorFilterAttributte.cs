using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
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
            var autorDto = resultado.Value as AutorGetDto;
            if (autorDto == null)
            {
                var autoresDto = resultado.Value as List<AutorGetDto>
                    ?? throw new ArgumentNullException("Se esperaba un instancia de AutorDto");
                autoresDto.ForEach(async autor => await generadorEnlaces.generarEnlaces(autor));
                resultado.Value = autoresDto;
            }
            else
            {
                await generadorEnlaces.generarEnlaces(autorDto);
            }
            await next();
        }
    }
}
