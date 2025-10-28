using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using restaurant_api.Data;
using restaurant_api.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using restaurant_api.DTOs;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public CategoriasController(ApplicationDbContext context) { _context = context; }

    [HttpGet]
    public async Task<IActionResult> GetCategorias()
    {
        return Ok(await _context.Categorias.ToListAsync());
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CrearCategoria(CrearCategoriaDto newCategoriaDto)
    {   

        var nuevaCategoria = new Categoria
        {
            Nombre = newCategoriaDto.Nombre
        };

        _context.Categorias.Add(nuevaCategoria);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCategorias), new { id = nuevaCategoria.Id }, nuevaCategoria);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> EditarCategoria(int id, [FromBody] EditarCategoriaDto editarCategoriaDto)
    {
        var categoriaExistente = await _context.Categorias.FindAsync(id);
        if (categoriaExistente == null)
        {
            return NotFound("Categoría no encontrada.");
        }
        if (!string.IsNullOrEmpty(editarCategoriaDto.Nombre))
        {
            categoriaExistente.Nombre = editarCategoriaDto.Nombre;
        }

        await _context.SaveChangesAsync();

        return Ok(categoriaExistente);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> BorrarCategoria(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null) return NotFound();
        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}