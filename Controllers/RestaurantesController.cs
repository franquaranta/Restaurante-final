using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using restaurant_api.DTOs;
using restaurant_api.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class RestaurantesController(IRestauranteService restauranteService) : BaseRestauranteController
{
    private readonly IRestauranteService _restauranteService = restauranteService;

    // GET: /api/restaurantes
    [HttpGet]
    public async Task<IActionResult> GetRestaurantes()
    {
        var restaurantes = await _restauranteService.GetAllAsync();
        return Ok(restaurantes);
    }

    // POST: /api/restaurantes/registrar
    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar(RegistroRestauranteDto dto)
    {
        var registrado = await _restauranteService.RegistrarAsync(dto);

        if (!registrado)
        {
            return BadRequest("El email ya está registrado.");
        }

        return StatusCode(201, "Restaurante registrado exitosamente.");
    }

    // GET: /api/restaurantes/{idRestaurante}/menu
    [HttpGet("{idRestaurante}/menu")]
    public async Task<IActionResult> GetMenu(int idRestaurante)
    {
        var menu = await _restauranteService.GetMenuAsync(idRestaurante);
        return Ok(menu);
    }

    [HttpGet("mi-cuenta")]
    [Authorize]
    public async Task<IActionResult> VerMiCuenta()
    {
        var restauranteId = GetRestauranteIdDesdeToken();

        if (restauranteId == null)
        {
            return Unauthorized("Token inválido.");
        }

        var restaurante = await _restauranteService.GetByIdAsync(restauranteId.Value);

        if (restaurante == null)
        {
            return NotFound("Restaurante no encontrado.");
        }

        return Ok(restaurante);
    }

    [HttpPut("mi-cuenta")]
    [Authorize]
    public async Task<IActionResult> EditarMiCuenta(RegistroRestauranteDto dto)
    {
        var restauranteId = GetRestauranteIdDesdeToken();

        if (restauranteId == null)
        {
            return Unauthorized("Token inválido.");
        }

        var restaurante = await _restauranteService.EditarCuentaAsync(restauranteId.Value, dto);

        if (restaurante == null)
        {
            return NotFound("Restaurante no encontrado.");
        }

        return Ok(restaurante);
    }

    [HttpDelete("mi-cuenta")]
    [Authorize]
    public async Task<IActionResult> BorrarMiCuenta()
    {
        var restauranteId = GetRestauranteIdDesdeToken();

        if (restauranteId == null)
        {
            return Unauthorized("Token inválido.");
        }

        var eliminado = await _restauranteService.EliminarCuentaAsync(restauranteId.Value);

        if (!eliminado)
        {
            return NotFound("Restaurante no encontrado.");
        }

        return Ok("Cuenta eliminada.");
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

        var productosAfectados = await _restauranteService.AumentarPreciosAsync(restauranteId.Value, dto);

        return Ok(new
        {
            mensaje = $"Se actualizaron los precios de {productosAfectados} productos.",
            porcentajeAplicado = $"{dto.Porcentaje}%"
        });
    }
}
