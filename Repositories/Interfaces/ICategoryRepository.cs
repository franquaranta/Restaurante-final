using restaurant_api.Models;

namespace restaurant_api.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task CreateCategory(Categoria nuevaCategoria);
    List<Categoria> GetCategories();
    Task<Categoria?> GetByIdAsync(int id);
    Task UpdateAsync(Categoria categoria);
    Task DeleteAsync(Categoria categoria);
}