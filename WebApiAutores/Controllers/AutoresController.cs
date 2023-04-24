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
using WebApiAutores.Utilidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "obtenerAutores")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttributte))]
        public async Task<ActionResult<List<AutorGetDto>>> Get([FromQuery] PaginacionDto paginacionDto)
        {
            var queryable = context.Autores.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var autores = await queryable.OrderBy(autor=>autor.Nombre).Paginar(paginacionDto).ToListAsync();
            return mapper.Map<List<AutorGetDto>>(autores);
        }
        [HttpGet("{id:int}", Name = "obtenerAutor")]//id tiene que ser int
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttributte))]
        public async Task<ActionResult<AutorDtoConLibros>> Get(int id, [FromHeader] string incluirHATEOAS)
        {
            var autor = await context.Autores.Include(a => a.AutoresLibros).ThenInclude(al => al.Libro).FirstOrDefaultAsync(x => x.Id == id);
            if (autor == null)
            {
                return NotFound();
            }
            var dto = mapper.Map<AutorDtoConLibros>(autor);
            return dto;
        }

        [HttpGet("{nombre}", Name = "obtenerAutorPorNombre")]
        [AllowAnonymous]
        public async Task<ActionResult<List<AutorGetDto>>> GetPorNombre(string nombre)
        {
            var autores = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();
            if (autores.Count == 0)
            {
                return NotFound();
            }

            return mapper.Map<List<AutorGetDto>>(autores);
        }

        [HttpPost(Name = "crearAutor")]
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

        [HttpPut("{id:int}", Name = "actualizarAutor")]
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

        [HttpDelete("{id:int}", Name = "borrarAutor")]
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
