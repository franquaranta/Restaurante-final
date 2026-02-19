using restaurant_api.DTOs;
using restaurant_api.Models;
using restaurant_api.Repositories.Interfaces;
using restaurant_api.Services.Interfaces;

namespace restaurant_api.Services.Implementations;

public class ProductoService(IProductoRepository productoRepository) : IProductoService
{
    private readonly IProductoRepository _productoRepository = productoRepository;

    public async Task<ProductoResponseDto?> GetDetalleAsync(int id)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto == null) return null;

        return new ProductoResponseDto
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
    }

    public async Task<List<ProductoResponseDto>> GetPorCategoriaAsync(int categoriaId)
    {
        var productos = await _productoRepository.GetByCategoriaAsync(categoriaId);
        return productos.Select(ToDto).ToList();
    }

    public async Task<List<ProductoResponseDto>> GetFavoritosAsync()
    {
        var productos = await _productoRepository.GetFavoritosAsync();
        return productos.Select(ToDto).ToList();
    }

    public async Task<List<ProductoResponseDto>> GetEnOfertaAsync()
    {
        var productos = await _productoRepository.GetEnOfertaAsync();
        return productos.Select(ToDto).ToList();
    }

    public async Task<ProductoResponseDto> CrearAsync(CrearProductoDto dto, int restauranteId)
    {
        var producto = new Producto
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            CategoriaId = dto.CategoriaId,
            EsFavorito = dto.EsFavorito,
            Descuento = dto.Descuento,
            TieneHappyHour = dto.TieneHappyHour,
            RestauranteId = restauranteId
        };

        await _productoRepository.AddAsync(producto);
        return ToDto(producto);
    }

    public async Task<ProductoResponseDto?> EditarAsync(int id, EditarProductoDto dto, int restauranteId)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto == null || producto.RestauranteId != restauranteId) return null;

        if (!string.IsNullOrEmpty(dto.Nombre)) producto.Nombre = dto.Nombre;
        if (!string.IsNullOrEmpty(dto.Descripcion)) producto.Descripcion = dto.Descripcion;
        if (dto.Precio.HasValue) producto.Precio = dto.Precio.Value;
        if (dto.CategoriaId.HasValue) producto.CategoriaId = dto.CategoriaId.Value;
        if (dto.EsFavorito.HasValue) producto.EsFavorito = dto.EsFavorito.Value;

        await _productoRepository.SaveChangesAsync();
        return ToDto(producto);
    }

    public async Task<bool> EliminarAsync(int id, int restauranteId)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto == null || producto.RestauranteId != restauranteId) return false;

        await _productoRepository.DeleteAsync(producto);
        return true;
    }

    public async Task<ProductoResponseDto?> ModificarDescuentoAsync(int id, decimal descuento, int restauranteId)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto == null || producto.RestauranteId != restauranteId) return null;

        producto.Descuento = descuento;
        await _productoRepository.SaveChangesAsync();
        return ToDto(producto);
    }

    public async Task<ProductoResponseDto?> ModificarHappyHourAsync(int id, bool tieneHappyHour, int restauranteId)
    {
        var producto = await _productoRepository.GetByIdAsync(id);
        if (producto == null || producto.RestauranteId != restauranteId) return null;

        producto.TieneHappyHour = tieneHappyHour;
        await _productoRepository.SaveChangesAsync();
        return ToDto(producto);
    }

    private static ProductoResponseDto ToDto(Producto p) => new()
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
    };
}
