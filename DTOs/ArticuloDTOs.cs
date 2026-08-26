namespace delivery.DTOs
{
    // El formato que devolvemos a la pantalla (con el ID)
    public class ArticuloGetDTO
    {
        public int CodArticulo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public short Stock { get; set; }
    }

    // El formato que le pedimos al usuario para crear uno nuevo (¡sin el ID!)
    public class ArticuloCreateDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public short Stock { get; set; }
    }
}