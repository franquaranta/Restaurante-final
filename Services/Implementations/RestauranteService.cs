using restaurant_api.DTOs;
using restaurant_api.Models;
using restaurant_api.Repositories.Interfaces;
using restaurant_api.Services.Interfaces;

namespace restaurant_api.Services.Implementations;

public class RestauranteService(
    IRestauranteRepository restauranteRepository,
    IProductoRepository productoRepository) : IRestauranteService
{
    private readonly IRestauranteRepository _restauranteRepository = restauranteRepository;
    private readonly IProductoRepository _productoRepository = productoRepository;

    public async Task<List<RestauranteResponseDto>> GetAllAsync()
    {
        var restaurantes = await _restauranteRepository.GetAllAsync();
        return restaurantes.Select(ToDto).ToList();
    }

    public async Task<RestauranteResponseDto?> GetByIdAsync(int id)
    {
        var restaurante = await _restauranteRepository.GetByIdAsync(id);
        return restaurante == null ? null : ToDto(restaurante);
    }

    public async Task<List<ProductoResponseDto>> GetMenuAsync(int restauranteId)
    {
        var restaurante = await _restauranteRepository.GetByIdAsync(restauranteId);
        if (restaurante == null)
            throw new KeyNotFoundException("Restaurante no encontrado.");

        var productos = await _productoRepository.GetByRestauranteAsync(restauranteId);
        return productos.Select(p => new ProductoResponseDto
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
        }).ToList();
    }

    public async Task<bool> RegistrarAsync(RegistroRestauranteDto dto)
    {
        if (dto.Email != null && await _restauranteRepository.EmailExisteAsync(dto.Email))
            return false;

        var restaurante = new Restaurante
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            Direccion = dto.Direccion,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await _restauranteRepository.AddAsync(restaurante);
        return true;
    }

    public async Task<RestauranteResponseDto?> EditarCuentaAsync(int restauranteId, RegistroRestauranteDto dto)
    {
        var restaurante = await _restauranteRepository.GetByIdAsync(restauranteId);
        if (restaurante == null) return null;

        if (!string.IsNullOrEmpty(dto.Nombre)) restaurante.Nombre = dto.Nombre;
        if (!string.IsNullOrEmpty(dto.Email)) restaurante.Email = dto.Email;
        if (!string.IsNullOrEmpty(dto.Direccion)) restaurante.Direccion = dto.Direccion;
        if (!string.IsNullOrEmpty(dto.Password))
            restaurante.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        await _restauranteRepository.SaveChangesAsync();
        return ToDto(restaurante);
    }

    public async Task<bool> EliminarCuentaAsync(int restauranteId)
    {
        var restaurante = await _restauranteRepository.GetByIdAsync(restauranteId);
        if (restaurante == null) return false;

        await _restauranteRepository.DeleteAsync(restaurante);
        return true;
    }

    public async Task<int> AumentarPreciosAsync(int restauranteId, AumentoPrecioDto dto)
    {
        var multiplicador = 1 + (dto.Porcentaje / 100);
        return await _productoRepository.AumentarPreciosAsync(restauranteId, multiplicador);
    }

    private static RestauranteResponseDto ToDto(Restaurante r) => new()
    {
        Id = r.Id,
        Nombre = r.Nombre,
        Email = r.Email,
        Direccion = r.Direccion
    };
}
