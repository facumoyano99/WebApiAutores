using AutoMapper;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WebApiAutores.Dtos;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }
       
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<AutorGetDto>>> Get()
        {
            var autor = await context.Autores.ToListAsync();
            return mapper.Map<List<AutorGetDto>>(autor);
        }
        [HttpGet("{id:int}", Name = "obtenerAutor")]//id tiene que ser int
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
        [AllowAnonymous]
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

            var autorGetDto = mapper.Map<AutorGetDto>(autor);

            return CreatedAtRoute("obtenerAutor", new { id = autor.Id }, autorGetDto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(AutorCreacionDto autorDto, int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorDto);
            autor.Id = id;

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
