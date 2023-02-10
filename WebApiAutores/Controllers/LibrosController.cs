using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutores.Dtos;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LibroDtoConAutores>> GetById(int id)
        {
            var libro =  await context.Libros.Include(l=>l.AutoresLibros).ThenInclude(al=>al.Autor).FirstOrDefaultAsync(x => x.Id == id);
            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();
            return mapper.Map<LibroDtoConAutores>(libro);
        }

        [HttpPost]
        public async Task<ActionResult> Post(LibroCreacionDto libroDto)
        {            
            if(libroDto.AutoresIds == null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }
            var autoresId = await context.Autores.Where(a => libroDto.AutoresIds.Contains(a.Id)).Select(x=>x.Id).ToListAsync();

            if (libroDto.AutoresIds.Count!=autoresId.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }
            var libro = mapper.Map<Libro>(libroDto);
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
            context.Add(libro);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
