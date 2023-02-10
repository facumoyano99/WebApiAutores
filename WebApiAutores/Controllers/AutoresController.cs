using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutores.Dtos;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AutoresController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AutorGetDto>>> Get()
        {
            var autor = await context.Autores.ToListAsync();
            return mapper.Map<List<AutorGetDto>>(autor);
        }
        [HttpGet("{id:int}")]//id tiene que ser int
        public async Task<ActionResult<AutorDtoConLibros>> Get(int id)
        {
            var autor = await context.Autores.Include(a => a.AutoresLibros).ThenInclude(al => al.Libro).FirstOrDefaultAsync(x => x.Id == id);
            if (autor == null)
            {
                return NotFound();
            }
            return mapper.Map<AutorDtoConLibros>(autor);
        }

        [HttpGet("{nombre}")]
        public async Task<ActionResult<List<AutorGetDto>>> Get(string nombre)
        {
            var autores = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();
            if (autores.Count == 0)
            {
                return NotFound();
            }

            return mapper.Map<List<AutorGetDto>>(autores);
        }

        [HttpPost]
        public async Task<ActionResult> Post(AutorCreacionDto autorDto)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Nombre.ToLower() == autorDto.Nombre.ToLower());

            if (existeAutor)
            {
                return BadRequest("Ya existe con autor con el nombre" + autorDto.Nombre);
            }

            var autor = mapper.Map<Autor>(autorDto);
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

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
