using restaurant_api.Models;

namespace restaurant_api.Repositories.Interfaces;

public interface IRestauranteRepository
{
    Task<Restaurante?> GetByIdAsync(int id);
    Task<Restaurante?> GetByEmailAsync(string email);
    Task<List<Restaurante>> GetAllAsync();
    Task<bool> EmailExisteAsync(string email);
    Task AddAsync(Restaurante restaurante);
    Task SaveChangesAsync();
    Task DeleteAsync(Restaurante restaurante);
}
