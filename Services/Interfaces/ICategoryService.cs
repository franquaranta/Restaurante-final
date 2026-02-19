using restaurant_api.DTOs;

namespace restaurant_api.Services.Interfaces;

public interface ICategoryService
{
    Task CreateCategory(SolicitudCrearCategoria solicitudCrearCategoria);
    List<CategoriaDTO> GetCategories();
    Task<CategoriaDTO?> UpdateCategoryAsync(int id, EditarCategoriaDto dto);
    Task<bool> DeleteCategoryAsync(int id);
}