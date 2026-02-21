using restaurant_api.DTOs;
using restaurant_api.Models;
using restaurant_api.Repositories.Interfaces;
using restaurant_api.Services.Interfaces;

namespace restaurant_api.Services.Implementations;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task CreateCategory(SolicitudCrearCategoria solicitudCrearCategoria, int restauranteId)
    {
        var nuevaCategoria = new Categoria
        {
            Nombre = solicitudCrearCategoria.Nombre,
            RestauranteId = restauranteId
        };

        await _categoryRepository.CreateCategory(nuevaCategoria);
    }

    public List<CategoriaResponseDto> GetCategories()
    {
        return _categoryRepository.GetCategories().Select(c => new CategoriaResponseDto
        {
            Id = c.Id,
            Nombre = c.Nombre,
            RestauranteId = c.RestauranteId
        }).ToList();
    }

    public async Task<CategoriaResponseDto?> UpdateCategoryAsync(int id, EditarCategoriaDto dto, int restauranteId)
    {
        var categoria = await _categoryRepository.GetByIdAsync(id);
        if (categoria == null || categoria.RestauranteId != restauranteId) return null;

        if (!string.IsNullOrEmpty(dto.Nombre)) categoria.Nombre = dto.Nombre;

        await _categoryRepository.UpdateAsync(categoria);
        return new CategoriaResponseDto { Id = categoria.Id, Nombre = categoria.Nombre, RestauranteId = categoria.RestauranteId };
    }

    public async Task<bool> DeleteCategoryAsync(int id, int restauranteId)
    {
        var categoria = await _categoryRepository.GetByIdAsync(id);
        if (categoria == null || categoria.RestauranteId != restauranteId) return false;

        await _categoryRepository.DeleteAsync(categoria);
        return true;
    }
}
