using restaurant_api.Models;

namespace restaurant_api.Repositories.Interfaces;

public interface IProductoRepository
{
    Task<Producto?> GetByIdAsync(int id);
    Task<List<Producto>> GetByCategoriaAsync(int categoriaId);
    Task<List<Producto>> GetFavoritosAsync();
    Task<List<Producto>> GetEnOfertaAsync();
    Task<List<Producto>> GetByRestauranteAsync(int restauranteId);
    Task AddAsync(Producto producto);
    Task SaveChangesAsync();
    Task DeleteAsync(Producto producto);
    Task<int> AumentarPreciosAsync(int restauranteId, decimal multiplicador);
}
