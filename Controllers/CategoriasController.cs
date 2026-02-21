using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using restaurant_api.DTOs;
using restaurant_api.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController(ICategoryService categoryService) : BaseRestauranteController
{
    private readonly ICategoryService _categoryService = categoryService;

    [HttpGet]
    public IActionResult GetCategorias()
    {
        List<CategoriaResponseDto> categoriesDtos = _categoryService.GetCategories();
        return Ok(categoriesDtos);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CrearCategoria(SolicitudCrearCategoria solicitudCrearCategoria)
    {
        var restauranteId = GetRestauranteIdDesdeToken();
        if (restauranteId == null) return Unauthorized();

        await _categoryService.CreateCategory(solicitudCrearCategoria, restauranteId.Value);
        return Ok("Categoría creada exitosamente.");
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> EditarCategoria(int id, [FromBody] EditarCategoriaDto editarCategoriaDto)
    {
        var restauranteId = GetRestauranteIdDesdeToken();
        if (restauranteId == null) return Unauthorized();

        var resultado = await _categoryService.UpdateCategoryAsync(id, editarCategoriaDto, restauranteId.Value);

        if (resultado == null)
        {
            return NotFound("Categoría no encontrada o no pertenece a este restaurante.");
        }

        return Ok(resultado);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> BorrarCategoria(int id)
    {
        var restauranteId = GetRestauranteIdDesdeToken();
        if (restauranteId == null) return Unauthorized();

        var eliminada = await _categoryService.DeleteCategoryAsync(id, restauranteId.Value);

        if (!eliminada)
        {
            return NotFound("Categoría no encontrada o no pertenece a este restaurante.");
        }

        return NoContent();
    }
}
