namespace delivery.DTOs
{
    // El formato que devolvemos a la pantalla (con el ID)
    public class ArticuloGetDTO
    {
        public int CodArticulo { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Costo { get; set; }
        public int Stock { get; set; }
        public string? UrlImagen { get; set; }
        public int CategoriaId { get; set; }
    }

    // El formato que le pedimos al usuario para crear uno nuevo (¡sin el ID!)
    public class ArticuloCreateDTO
    {
        public string Descripcion { get; set; }
        public decimal Costo { get; set; }
        public int Stock { get; set; }
        public string UrlImagen { get; set; }
        public int CategoriaId { get; set; }
    }
}