namespace restaurant_api.DTOs
{
    public class EditarProductoDto
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal? Precio { get; set; }
        public int? CategoriaId { get; set; }  
        public bool? EsFavorito { get; set; }
    }
}