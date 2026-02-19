using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using restaurant_api.DTOs;
using restaurant_api.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class ProductosController(IProductoService productoService) : BaseRestauranteController
{
    private readonly IProductoService _productoService = productoService;

    // Endpoints públicos

    // GET: /api/productos/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductoDetalle(int id)
    {
        var producto = await _productoService.GetDetalleAsync(id);

        if (producto == null)
        {
            return NotFound();
        }

        return Ok(producto);
    }

    // GET: /api/productos/filtrar?categoriaId=2
    [HttpGet("filtrar")]
    public async Task<IActionResult> GetProductosPorCategoria([FromQuery] int categoriaId)
    {
        var productos = await _productoService.GetPorCategoriaAsync(categoriaId);
        return Ok(productos);
    }

    [HttpGet("favoritos")]
    public async Task<IActionResult> GetProductosFavoritos()
    {
        var productos = await _productoService.GetFavoritosAsync();
        return Ok(productos);
    }

    [HttpGet("ofertas")]
    public async Task<IActionResult> GetProductosEnOferta()
    {
        var productos = await _productoService.GetEnOfertaAsync();
        return Ok(productos);
    }

    // Endpoints privados (accedidos por el restaurante)

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CrearProducto([FromBody] CrearProductoDto dto)
    {
        var restauranteId = GetRestauranteIdDesdeToken();

        if (restauranteId == null)
        {
            return Unauthorized("Token inválido.");
        }

        var producto = await _productoService.CrearAsync(dto, restauranteId.Value);
        return CreatedAtAction(nameof(GetProductoDetalle), new { id = producto.Id }, producto);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> EditarProducto(int id, [FromBody] EditarProductoDto dto)
    {
        var restauranteId = GetRestauranteIdDesdeToken();

        if (restauranteId == null)
        {
            return Unauthorized("Token inválido.");
        }

        var producto = await _productoService.EditarAsync(id, dto, restauranteId.Value);

        if (producto == null)
        {
            return NotFound("Producto no encontrado o sin permisos.");
        }

        return Ok(producto);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> BorrarProducto(int id)
    {
        var restauranteId = GetRestauranteIdDesdeToken();

        if (restauranteId == null)
        {
            return Unauthorized("Token inválido.");
        }

        var eliminado = await _productoService.EliminarAsync(id, restauranteId.Value);

        if (!eliminado)
        {
            return NotFound("Producto no encontrado o sin permisos.");
        }

        return NoContent();
    }

    [HttpPatch("{id}/descuento")]
    [Authorize]
    public async Task<IActionResult> ModificarDescuento(int id, [FromBody] decimal nuevoDescuento)
    {
        var restauranteId = GetRestauranteIdDesdeToken();

        if (restauranteId == null)
        {
            return Unauthorized("Token inválido.");
        }

        var producto = await _productoService.ModificarDescuentoAsync(id, nuevoDescuento, restauranteId.Value);

        if (producto == null)
        {
            return NotFound("Producto no encontrado o sin permisos.");
        }

        return Ok(producto);
    }

    [HttpPatch("{id}/happyhour")]
    [Authorize]
    public async Task<IActionResult> ModificarHappyHour(int id, [FromBody] bool tieneHappyHour)
    {
        var restauranteId = GetRestauranteIdDesdeToken();

        if (restauranteId == null)
        {
            return Unauthorized("Token inválido.");
        }

        var producto = await _productoService.ModificarHappyHourAsync(id, tieneHappyHour, restauranteId.Value);

        if (producto == null)
        {
            return NotFound("Producto no encontrado o sin permisos.");
        }

        return Ok(producto);
    }
}
