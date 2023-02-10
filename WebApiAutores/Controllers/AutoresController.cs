using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutores.Entidades;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public IServicio Servicio { get; }

        public AutoresController(ApplicationDbContext context, IServicio servicio)
        {
            this.context = context;
            Servicio = servicio;
        }

        [HttpGet]
        public async  Task<ActionResult<List<Autor>>> Get()
        {
            return await context.Autores.Include(x=>x.Libros).ToListAsync();
        }
        [HttpGet]
        [HttpGet("listado")] //api/autores/listado
        [HttpGet("/listado")] //listado
        public async Task<ActionResult<List<Autor>>> GetListado()
        {
            return await context.Autores.Include(x => x.Libros).ToListAsync();
        }
        [HttpGet("listado")] //api/autores/primero
        public async Task<ActionResult<Autor>> PrimerAutor()
        {
            return await context.Autores.FirstOrDefaultAsync();
        }
        [HttpGet("{id:int}/{param2=persona}")]//id tiene que ser int, param2 si es nulo es igual a persona.
        public async Task<ActionResult<Autor>> Get(int id, string param2)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            if(autor == null)
            {
                return NotFound();
            }
            return autor;
        }

        [HttpPost]
        public async Task<ActionResult> Post (Autor autor)
        {
            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(Autor autor, int id)
        {
            if (autor.Id != id)
            {
                return BadRequest("No coincide");
            }
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }
            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id});
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
