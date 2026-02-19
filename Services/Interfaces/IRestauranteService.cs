using restaurant_api.DTOs;

namespace restaurant_api.Services.Interfaces;

public interface IRestauranteService
{
    Task<List<RestauranteResponseDto>> GetAllAsync();
    Task<RestauranteResponseDto?> GetByIdAsync(int id);
    Task<List<ProductoResponseDto>> GetMenuAsync(int restauranteId);
    Task<bool> RegistrarAsync(RegistroRestauranteDto dto);
    Task<RestauranteResponseDto?> EditarCuentaAsync(int restauranteId, RegistroRestauranteDto dto);
    Task<bool> EliminarCuentaAsync(int restauranteId);
    Task<int> AumentarPreciosAsync(int restauranteId, AumentoPrecioDto dto);
}
