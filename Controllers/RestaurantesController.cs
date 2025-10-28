using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurant_api.Data;
using restaurant_api.Models;
using restaurant_api.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class RestaurantesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RestaurantesController(ApplicationDbContext context)
    {
        _context = context;
    }

private int? GetRestauranteIdDesdeToken()
{
    var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

    if (idClaim != null && int.TryParse(idClaim.Value, out int restauranteId))
    {
        return restauranteId;
    }

    return null;
}

    [HttpPut("mi-cuenta")]
    [Authorize]
    public async Task<IActionResult> EditarMiCuenta(RegistroRestauranteDto dto) 
    {
        var restauranteId = GetRestauranteIdDesdeToken();
        var restaurante = await _context.Restaurantes.FindAsync(restauranteId);

        if (restaurante == null) return NotFound();

if (!string.IsNullOrEmpty(dto.Nombre))
        {
            restaurante.Nombre = dto.Nombre;
        }

        if (!string.IsNullOrEmpty(dto.Email))
        {
            restaurante.Email = dto.Email;
        }
        
        if (!string.IsNullOrEmpty(dto.Direccion))
        {
            restaurante.Direccion = dto.Direccion;
        }

        // Opcional: Actualizar contraseña si se proveyó una nueva
        if (!string.IsNullOrEmpty(dto.Password))
        {
            restaurante.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        await _context.SaveChangesAsync();
        return Ok("Cuenta actualizada.");
    }

    [HttpGet("mi-cuenta")]
    [Authorize]
    public async Task<IActionResult> VerMiCuenta()
    {
        var restauranteId = GetRestauranteIdDesdeToken();
        var restaurante = await _context.Restaurantes.FindAsync(restauranteId);

        if (restaurante == null)
        {
            return NotFound("Restaurante no encontrado.");
        }

        var datosCuenta = new 
        {
            restaurante.Id,
            restaurante.Nombre,
            restaurante.Email,
            restaurante.Direccion
        };

        return Ok(datosCuenta);
    }

    [HttpDelete("mi-cuenta")]
    [Authorize]
    public async Task<IActionResult> BorrarMiCuenta()
    {
        var restauranteId = GetRestauranteIdDesdeToken();
        var restaurante = await _context.Restaurantes.FindAsync(restauranteId);

        if (restaurante == null) return NotFound();

        _context.Restaurantes.Remove(restaurante);
        await _context.SaveChangesAsync();
        return Ok("Cuenta eliminada.");
    }

    // GET: /api/restaurantes
    [HttpGet]
    public async Task<IActionResult> GetRestaurantes()
    {
        var restaurantes = await _context.Restaurantes
            .Select(r => new {
                r.Id,
                r.Nombre,
                r.Email,
                r.Direccion
            })
            .ToListAsync();
            
        return Ok(restaurantes);
    }

    // POST: /api/restaurantes/registrar
    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar(RegistroRestauranteDto dto)
    {
        var emailExiste = await _context.Restaurantes.AnyAsync(r => r.Email == dto.Email);
        if (emailExiste)
        {
            return BadRequest("El email ya está registrado.");
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var nuevoRestaurante = new Restaurante
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            Direccion = dto.Direccion,
            PasswordHash = passwordHash
        };

        _context.Restaurantes.Add(nuevoRestaurante);
        await _context.SaveChangesAsync();

        return StatusCode(201, "Restaurante registrado exitosamente.");
    }

    // GET: /api/restaurantes/5/menu
[HttpGet("{idRestaurante}/menu")]
    public async Task<IActionResult> GetMenu(int idRestaurante)
    {
        var productosDelMenu = await _context.Productos
            .Where(p => p.RestauranteId == idRestaurante)
            .Select(p => new ProductoResponseDto 
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                EsFavorito = p.EsFavorito,
                Descuento = p.Descuento,
                TieneHappyHour = p.TieneHappyHour,
                CategoriaId = p.CategoriaId,
                RestauranteId = p.RestauranteId
            })
            .ToListAsync();
            
        return Ok(productosDelMenu);
    }


    [HttpPatch("mi-menu/aumentar-precios")]
    [Authorize]
    public async Task<IActionResult> AumentarPreciosMenu([FromBody] AumentoPrecioDto dto)
    {
        var restauranteId = GetRestauranteIdDesdeToken();
        if (restauranteId == null)
        {
            return Unauthorized("Token inválido.");
        }

        if (dto.Porcentaje <= 0)
        {
            return BadRequest("El porcentaje debe ser un número positivo (ej: 15.5 para 15.5%).");
        }

        var multiplicador = 1 + (dto.Porcentaje / 100);

        int productosAfectados = await _context.Productos
            .Where(p => p.RestauranteId == restauranteId.Value)
            .ExecuteUpdateAsync(s => s.SetProperty(
                p => p.Precio,              
                p => p.Precio * multiplicador
            ));

        return Ok(new { 
            mensaje = $"Se actualizaron los precios de {productosAfectados} productos.",
            porcentajeAplicado = $"{dto.Porcentaje}%"
        });
    }
}