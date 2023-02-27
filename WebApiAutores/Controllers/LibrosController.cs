using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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

        [HttpGet("{id:int}", Name = "obtenerLibro")]
        public async Task<ActionResult<LibroDtoConAutores>> GetById(int id)
        {
            var libro = await context.Libros.Include(l => l.AutoresLibros).ThenInclude(al => al.Autor).FirstOrDefaultAsync(x => x.Id == id);
            if (libro==null)
            {
                return NotFound();
            }
            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();
            return mapper.Map<LibroDtoConAutores>(libro);
        }

        [HttpPost]
        public async Task<ActionResult> Post(LibroCreacionDto libroDto)
        {
            if (libroDto.AutoresIds == null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }
            var autoresId = await context.Autores.Where(a => libroDto.AutoresIds.Contains(a.Id)).Select(x => x.Id).ToListAsync();

            if (libroDto.AutoresIds.Count != autoresId.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }
            var libro = mapper.Map<Libro>(libroDto);
            asignarOrdenAutores(libro);
            context.Add(libro);
            await context.SaveChangesAsync();

            var libroGetDto = mapper.Map<LibroGetDto>(libro);
            return CreatedAtRoute("obtenerLibro", new { id = libro.Id }, libroGetDto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, LibroCreacionDto libroDto)
        {
            var libroBD = await context.Libros.Include(x => x.AutoresLibros).FirstOrDefaultAsync(x => x.Id == id);

            if (libroBD == null)
            {
                return NotFound();
            }

            libroBD = mapper.Map(libroDto, libroBD);
            asignarOrdenAutores(libroBD);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDto> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }
            var libroBD = await context.Libros.Include(x => x.AutoresLibros).FirstOrDefaultAsync(x => x.Id == id);

            if (libroBD == null)
            {
                return NotFound();
            }

            var libroDto = mapper.Map<LibroPatchDto>(libroBD);
            patchDocument.ApplyTo(libroDto, ModelState);

            var esValido = TryValidateModel(libroDto);
            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(libroDto, libroBD);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Libros.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Libro() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }

        private void asignarOrdenAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }
    }
}
