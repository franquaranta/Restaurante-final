namespace restaurant_api.DTOs
{
    public class CategoriaResponseDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public int RestauranteId { get; set; }
    }
}
