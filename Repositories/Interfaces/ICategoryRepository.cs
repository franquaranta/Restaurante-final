using restaurant_api.Models;

namespace restaurant_api.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task CreateCategory(Categoria nuevaCategoria);
    List<Categoria> GetCategories();
}