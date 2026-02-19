using restaurant_api.Data;
using restaurant_api.Models;
using restaurant_api.Repositories.Interfaces;

namespace restaurant_api.Repositories.Implementations;

public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task CreateCategory(Categoria nuevaCategoria)
    {
        _context.Categorias.Add(nuevaCategoria);

        await _context.SaveChangesAsync();
    }

    public List<Categoria> GetCategories()
    {
        return _context.Categorias.ToList();
    }

    public async Task<Categoria?> GetByIdAsync(int id)
    {
        return await _context.Categorias.FindAsync(id);
    }

    public async Task UpdateAsync(Categoria categoria)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Categoria categoria)
    {
        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();
    }
}