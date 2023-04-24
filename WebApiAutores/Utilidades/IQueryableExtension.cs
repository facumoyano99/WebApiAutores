using System.Linq;
using WebApiAutores.Dtos;

namespace WebApiAutores.Utilidades
{
    public static class IQueryableExtension
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDto paginacionDto)
        {
            return queryable.Skip((paginacionDto.Pagina - 1) * paginacionDto.RecordsPorPagina)
                .Take(paginacionDto.RecordsPorPagina);
        }
    }
}
