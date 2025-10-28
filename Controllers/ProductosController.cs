using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurant_api.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using restaurant_api.Models;
using restaurant_api.DTOs;
using Microsoft.Data.Sqlite;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Endpoints públicos   

    // GET: /api/productos/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductoDetalle(int id)
    {
        var producto = await _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Restaurante)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (producto == null)
        {
            return NotFound();
        }
        
        var productoDto = new {
            producto.Id,
            producto.Nombre,
            producto.Descripcion,
            producto.Precio,
            producto.EsFavorito,
            producto.Descuento,
            producto.TieneHappyHour,
            producto.CategoriaId,

            Categoria = new {
                producto.Categoria.Id,
                producto.Categoria.Nombre
            },
            producto.RestauranteId,
            Restaurante = new {
                producto.Restaurante.Id,
                producto.Restaurante.Nombre,
                producto.Restaurante.Email
            }
        };

        return Ok(productoDto);
    }

    // GET: /api/productos/filtrar?categoriaId=2
    [HttpGet("filtrar")]
    public async Task<IActionResult> GetProductosPorCategoria([FromQuery] int categoriaId)
    {
        var productos = await _context.Productos
            .Where(p => p.CategoriaId == categoriaId)
            .ToListAsync();
            
        return Ok(productos);
    }

    [HttpGet("favoritos")]
    public async Task<IActionResult> GetProductosFavoritos()
    {
        var productos = await _context.Productos
            .Where(p => p.EsFavorito == true)
            .ToListAsync();
            
        return Ok(productos);
    }

    [HttpGet("ofertas")]
    public async Task<IActionResult> GetProductosEnOferta()
    {
        var productos = await _context.Productos
            .Where(p => p.Descuento > 0 || p.TieneHappyHour == true)
            .ToListAsync();
            
        return Ok(productos);
    }


    // Endpoints privados (accedidos por el restaurante o dueño)


    [HttpPost]
    [Authorize] 
    public async Task<IActionResult> CrearProducto([FromBody] CrearProductoDto dto) 
    {
        var restauranteId = GetRestauranteIdDesdeToken();

        if (restauranteId == null)
        {
            return Unauthorized("Token inválido.");
        }

        var nuevoProducto = new Producto {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            CategoriaId = dto.CategoriaId,
            EsFavorito = dto.EsFavorito,
            Descuento = dto.Descuento,
            TieneHappyHour = dto.TieneHappyHour,
            RestauranteId = restauranteId.Value
        };

        _context.Productos.Add(nuevoProducto);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqliteException sqliteEx && sqliteEx.SqliteErrorCode == 19)
            {
                return BadRequest("Error: La categoría seleccionada no existe.");
            }
            return StatusCode(500, "Error interno al guardar el producto.");
        }

        var responseDto = new ProductoResponseDto
        {
            Id = nuevoProducto.Id,
            Nombre = nuevoProducto.Nombre,
            Descripcion = nuevoProducto.Descripcion,
            Precio = nuevoProducto.Precio,
            EsFavorito = nuevoProducto.EsFavorito,
            Descuento = nuevoProducto.Descuento,
            TieneHappyHour = nuevoProducto.TieneHappyHour,
            CategoriaId = nuevoProducto.CategoriaId,
            RestauranteId = nuevoProducto.RestauranteId
        };

        return CreatedAtAction(nameof(GetProductoDetalle), new { id = responseDto.Id }, responseDto);

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

        var productoExistente = await _context.Productos.FindAsync(id);

        if (productoExistente == null)
        {
            return NotFound("Producto no encontrado.");
        }

        if (productoExistente.RestauranteId != restauranteId.Value)
        {
            return Forbid("No tienes permiso para editar este producto.");
        }

        if (!string.IsNullOrEmpty(dto.Nombre))
        {
            productoExistente.Nombre = dto.Nombre;
        }
        
        if (!string.IsNullOrEmpty(dto.Descripcion))
        {
            productoExistente.Descripcion = dto.Descripcion;
        }

        if (dto.Precio.HasValue) 
        {
            productoExistente.Precio = dto.Precio.Value;
        }

        if (dto.CategoriaId.HasValue) 
        {
            productoExistente.CategoriaId = dto.CategoriaId.Value;
        }

        if (dto.EsFavorito.HasValue) 
        {
            productoExistente.EsFavorito = dto.EsFavorito.Value;
        }
        

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is Microsoft.Data.Sqlite.SqliteException sqliteEx && sqliteEx.SqliteErrorCode == 19)
            {
                return BadRequest("Error: La categoría seleccionada no existe.");
            }
            return StatusCode(500, "Error interno al actualizar el producto.");
        }

        var responseDto = new ProductoResponseDto
        {
            Id = productoExistente.Id,
            Nombre = productoExistente.Nombre,
            Descripcion = productoExistente.Descripcion,
            Precio = productoExistente.Precio,
            EsFavorito = productoExistente.EsFavorito,
            Descuento = productoExistente.Descuento,
            TieneHappyHour = productoExistente.TieneHappyHour,
            CategoriaId = productoExistente.CategoriaId,
            RestauranteId = productoExistente.RestauranteId
        };

        return Ok(responseDto);
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

        var producto = await _context.Productos.FindAsync(id);

        if (producto == null)
        {
            return NotFound();
        }

        if (producto.RestauranteId != restauranteId)
        {
            return Forbid("No tienes permiso para borrar este producto.");
        }

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();
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

        var producto = await _context.Productos.FindAsync(id);

        if (producto == null)
        {
            return NotFound("Producto no encontrado.");
        }

        if (producto.RestauranteId != restauranteId.Value)
        {
            return Forbid("No tienes permiso para modificar este producto.");
        }

        producto.Descuento = nuevoDescuento;
        await _context.SaveChangesAsync();
        
        var responseDto = new ProductoResponseDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            EsFavorito = producto.EsFavorito,
            Descuento = producto.Descuento,
            TieneHappyHour = producto.TieneHappyHour,
            CategoriaId = producto.CategoriaId,
            RestauranteId = producto.RestauranteId
        };
        
        return Ok(responseDto);
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

        var producto = await _context.Productos.FindAsync(id);

        if (producto == null)
        {
            return NotFound("Producto no encontrado.");
        }

        if (producto.RestauranteId != restauranteId.Value)
        {
            return Forbid("No tienes permiso para modificar este producto.");
        }

        producto.TieneHappyHour = tieneHappyHour;
        await _context.SaveChangesAsync();
        
        var responseDto = new ProductoResponseDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            EsFavorito = producto.EsFavorito,
            Descuento = producto.Descuento,
            TieneHappyHour = producto.TieneHappyHour,
            CategoriaId = producto.CategoriaId,
            RestauranteId = producto.RestauranteId
        };
        
        return Ok(responseDto);
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





}