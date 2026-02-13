using restaurant_api.DTOs;
using restaurant_api.Models;
using restaurant_api.Repositories.Interfaces;
using restaurant_api.Services.Interfaces;

namespace restaurant_api.Services.Implementations;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task CreateCategory(SolicitudCrearCategoria solicitudCrearCategoria)
    {
        var nuevaCategoria = new Categoria
        {
            Nombre = solicitudCrearCategoria.Nombre
        };

        await _categoryRepository.CreateCategory(nuevaCategoria);
    }

    public List<CategoriaDTO> GetCategories()
    {
        return _categoryRepository.GetCategories().Select(c => new CategoriaDTO()
        {
            Nombre = c.Nombre,
        }).ToList();
    }
}