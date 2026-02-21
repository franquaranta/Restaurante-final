namespace restaurant_api.Models {
public class Producto
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public bool EsFavorito { get; set; }
    public decimal Descuento { get; set; }
    public bool TieneHappyHour { get; set; }

    // Relaciones
    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public int RestauranteId { get; set; }
    public Restaurante Restaurante { get; set; } = null!;
}
}