namespace restaurant_api.DTOs
{
    public class ProductoResponseDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public bool EsFavorito { get; set; }
        public decimal Descuento { get; set; }
        public bool TieneHappyHour { get; set; }
        
        public int CategoriaId { get; set; }
        public int RestauranteId { get; set; }
    }
}