using restaurant_api.DTOs;

namespace restaurant_api.Services.Interfaces;

public interface ICategoryService
{
    Task CreateCategory(SolicitudCrearCategoria solicitudCrearCategoria, int restauranteId);
    List<CategoriaResponseDto> GetCategories();
    Task<CategoriaResponseDto?> UpdateCategoryAsync(int id, EditarCategoriaDto dto, int restauranteId);
    Task<bool> DeleteCategoryAsync(int id, int restauranteId);
}