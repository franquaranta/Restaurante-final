namespace restaurant_api.DTOs
{
    public class CrearProductoDto
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int CategoriaId { get; set; }

        public bool EsFavorito { get; set; } = false;
        public decimal Descuento { get; set; } = 0;
        public bool TieneHappyHour { get; set; } = false;
    }
}