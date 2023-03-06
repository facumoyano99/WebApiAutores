using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiAutores.Dtos;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers
{
    [Route("api/cuentas")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;

        public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager, IDataProtectionProvider dataProtectionProvider, HashService hashService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            dataProtector = dataProtectionProvider.CreateProtector("valor_unico");
        }

        [HttpGet("{textoPlano}")]
        public ActionResult RealizarHash(string textoPlano)
        {
            var resultado1 = hashService.Hash(textoPlano);
            var resultado2 = hashService.Hash(textoPlano);

            return Ok(new
            {
                textoPlano = textoPlano,
                Hash1 = resultado1,
                Hash2 = resultado2,
            });
        }

        [HttpGet("encriptar")]
        public ActionResult Encriptar()
        {
            string textoPlano = "Feplie Gavilan";
            var textoCrifrado = dataProtector.Protect(textoPlano);
            var textDescencriptado = dataProtector.Unprotect(textoCrifrado);

            return Ok(new
            {
                textoPlano = textoPlano,
                textoCrifrado = textoCrifrado,
                textDescencriptado = textDescencriptado
            });
        }
        [HttpGet("encriptarPorTiempo")]
        public ActionResult EncriptarPorTiempo()
        {
            var tiempo = dataProtector.ToTimeLimitedDataProtector();

            string textoPlano = "Feplie Gavilan";
            var textoCrifrado = tiempo.Protect(textoPlano, lifetime: TimeSpan.FromSeconds(5));
            Thread.Sleep(6000);
            var textDescencriptado = dataProtector.Unprotect(textoCrifrado);

            return Ok(new
            {
                textoPlano = textoPlano,
                textoCrifrado = textoCrifrado,
                textDescencriptado = textDescencriptado
            });
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credenciales)
        {
            var usuario = new IdentityUser { UserName = credenciales.Email, Email = credenciales.Email };
            var resultado = await userManager.CreateAsync(usuario, credenciales.Password);
            if (resultado.Succeeded)
            {
                return await ConstruirToken(credenciales);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credenciales)
        {
            var resultado = await signInManager.PasswordSignInAsync(credenciales.Email, credenciales.Password, isPersistent: false, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credenciales);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }


        [HttpGet("RenovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutenticacion>> Renovar()
        {
            Claim emailClaim = HttpContext.User.Claims.Where(c => c.Type == "email").FirstOrDefault();
            string email = emailClaim.Value;
            CredencialesUsuario credUsu = new()
            {
                Email = email
            };
            return await ConstruirToken(credUsu);
        }

        private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credenciales)
        {
            var claims = new List<Claim>()
            {
                new  Claim("email", credenciales.Email),

            };

            var usu = await userManager.FindByEmailAsync(credenciales.Email);
            var claimsBD = await userManager.GetClaimsAsync(usu);

            claims.AddRange(claimsBD);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: creds);

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiration
            };
        }

        [HttpPost("HacerAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDto adminDto)
        {
            var usu = await userManager.FindByEmailAsync(adminDto.Email);
            await userManager.AddClaimAsync(usu, new Claim("esAdmin", "1"));
            return NoContent();
        }
        [HttpPost("RemoverAdmin")]
        public async Task<ActionResult> RemoverAdmin(EditarAdminDto adminDto)
        {
            var usu = await userManager.FindByEmailAsync(adminDto.Email);
            await userManager.RemoveClaimAsync(usu, new Claim("esAdmin", "1"));
            return NoContent();
        }
    }
}
