using System.Collections.Generic;

namespace delivery.DTOs
{
    public class DetallePromoCreateDTO
    {
        public int CodArticulo { get; set; }
        public short Cantidad { get; set; }
    }

    public class PromoCreateDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string? UrlImagen { get; set; }
        public decimal PrecioVenta { get; set; }

        // Acá recibimos la lista de artículos elegidos desde el frontend
        public List<DetallePromoCreateDTO> Articulos { get; set; } = new();
    }
}