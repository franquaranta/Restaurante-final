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
    public IActionResult GetCategorias()
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

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> EditarCategoria(int id, [FromBody] EditarCategoriaDto editarCategoriaDto)
    {
        var resultado = await _categoryService.UpdateCategoryAsync(id, editarCategoriaDto);

        if (resultado == null)
        {
            return NotFound("Categoría no encontrada.");
        }

        return Ok(resultado);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> BorrarCategoria(int id)
    {
        var eliminada = await _categoryService.DeleteCategoryAsync(id);

        if (!eliminada)
        {
            return NotFound("Categoría no encontrada.");
        }

        return NoContent();
    }
}
