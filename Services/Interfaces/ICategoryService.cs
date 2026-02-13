using restaurant_api.DTOs;

namespace restaurant_api.Services.Interfaces;

public interface ICategoryService
{
    Task CreateCategory(SolicitudCrearCategoria solicitudCrearCategoria);
    public List<CategoriaDTO> GetCategories();
}