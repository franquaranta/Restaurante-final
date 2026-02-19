using Microsoft.EntityFrameworkCore;
using restaurant_api.Data;
using restaurant_api.Models;
using restaurant_api.Repositories.Interfaces;

namespace restaurant_api.Repositories.Implementations;

public class ProductoRepository(ApplicationDbContext context) : IProductoRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Producto?> GetByIdAsync(int id)
    {
        return await _context.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Restaurante)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Producto>> GetByCategoriaAsync(int categoriaId)
    {
        return await _context.Productos
            .Where(p => p.CategoriaId == categoriaId)
            .ToListAsync();
    }

    public async Task<List<Producto>> GetFavoritosAsync()
    {
        return await _context.Productos
            .Where(p => p.EsFavorito)
            .ToListAsync();
    }

    public async Task<List<Producto>> GetEnOfertaAsync()
    {
        return await _context.Productos
            .Where(p => p.Descuento > 0 || p.TieneHappyHour)
            .ToListAsync();
    }

    public async Task<List<Producto>> GetByRestauranteAsync(int restauranteId)
    {
        return await _context.Productos
            .Where(p => p.RestauranteId == restauranteId)
            .ToListAsync();
    }

    public async Task AddAsync(Producto producto)
    {
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Producto producto)
    {
        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();
    }

    public async Task<int> AumentarPreciosAsync(int restauranteId, decimal multiplicador)
    {
        return await _context.Productos
            .Where(p => p.RestauranteId == restauranteId)
            .ExecuteUpdateAsync(s => s.SetProperty(
                p => p.Precio,
                p => p.Precio * multiplicador
            ));
    }
}
