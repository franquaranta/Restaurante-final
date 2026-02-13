using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using restaurant_api.DTOs;
using restaurant_api.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController(ICategoryService categoryService) : ControllerBase
{
    private readonly ICategoryService _categoryService = categoryService;

    [HttpGet]
    public async Task<IActionResult> GetCategorias()
    {
        List<CategoriaDTO> categoriesDtos = _categoryService.GetCategories();

        return Ok(categoriesDtos);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CrearCategoria(SolicitudCrearCategoria solicitudCrearCategoria)
    {
        await _categoryService.CreateCategory(solicitudCrearCategoria);

        return Ok();
    }

    //[HttpPut("{id}")]
    //[Authorize]
    //public async Task<IActionResult> EditarCategoria(int id, [FromBody] EditarCategoriaDto editarCategoriaDto)
    //{
    //    var categoriaExistente = await _context.Categorias.FindAsync(id);
    //    if (categoriaExistente == null)
    //    {
    //        return NotFound("Categoría no encontrada.");
    //    }
    //    if (!string.IsNullOrEmpty(editarCategoriaDto.Nombre))
    //    {
    //        categoriaExistente.Nombre = editarCategoriaDto.Nombre;
    //    }

    //    await _context.SaveChangesAsync();

    //    return Ok(categoriaExistente);
    //}

    //[HttpDelete("{id}")]
    //[Authorize]
    //public async Task<IActionResult> BorrarCategoria(int id)
    //{
    //    var categoria = await _context.Categorias.FindAsync(id);
    //    if (categoria == null) return NotFound();
    //    _context.Categorias.Remove(categoria);
    //    await _context.SaveChangesAsync();
    //    return NoContent();
    //}
}