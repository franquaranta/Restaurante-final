using restaurant_api.DTOs;

namespace restaurant_api.Services.Interfaces;

public interface IProductoService
{
    Task<ProductoResponseDto?> GetDetalleAsync(int id);
    Task<List<ProductoResponseDto>> GetPorCategoriaAsync(int categoriaId);
    Task<List<ProductoResponseDto>> GetFavoritosAsync();
    Task<List<ProductoResponseDto>> GetEnOfertaAsync();
    Task<ProductoResponseDto> CrearAsync(CrearProductoDto dto, int restauranteId);
    Task<ProductoResponseDto?> EditarAsync(int id, EditarProductoDto dto, int restauranteId);
    Task<bool> EliminarAsync(int id, int restauranteId);
    Task<ProductoResponseDto?> ModificarDescuentoAsync(int id, decimal descuento, int restauranteId);
    Task<ProductoResponseDto?> ModificarHappyHourAsync(int id, bool tieneHappyHour, int restauranteId);
}
