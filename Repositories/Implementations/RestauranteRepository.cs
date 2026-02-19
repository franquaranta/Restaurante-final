using Microsoft.EntityFrameworkCore;
using restaurant_api.Data;
using restaurant_api.Models;
using restaurant_api.Repositories.Interfaces;

namespace restaurant_api.Repositories.Implementations;

public class RestauranteRepository(ApplicationDbContext context) : IRestauranteRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Restaurante?> GetByIdAsync(int id)
    {
        return await _context.Restaurantes.FindAsync(id);
    }

    public async Task<Restaurante?> GetByEmailAsync(string email)
    {
        return await _context.Restaurantes
            .FirstOrDefaultAsync(r => r.Email == email);
    }

    public async Task<List<Restaurante>> GetAllAsync()
    {
        return await _context.Restaurantes.ToListAsync();
    }

    public async Task<bool> EmailExisteAsync(string email)
    {
        return await _context.Restaurantes.AnyAsync(r => r.Email == email);
    }

    public async Task AddAsync(Restaurante restaurante)
    {
        _context.Restaurantes.Add(restaurante);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Restaurante restaurante)
    {
        _context.Restaurantes.Remove(restaurante);
        await _context.SaveChangesAsync();
    }
}
