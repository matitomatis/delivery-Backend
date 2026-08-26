using System.Collections.Generic;

namespace delivery.DTOs
{
    // Este es el "formulario" principal que va a ver el cliente
    public class PedidoCreateDTO
    {
        public int CodCliente { get; set; }
        public int CodFormaPago { get; set; }
        public int CodTipoEnvio { get; set; }

        // Fijate que acá NO pusimos ni Fecha ni Total. ¡Están prohibidos para el usuario!

        // Una lista con los detalles de lo que está comprando
        public List<DetallePedidoCreateDTO> Detalles { get; set; } = new List<DetallePedidoCreateDTO>();
    }

    // Este es el "formulario" para cada empanada o artículo que agregue
    public class DetallePedidoCreateDTO
    {
        public int? CodArticulo { get; set; }

        // Ayer peleamos con Entity Framework para que el código de promo sea opcional, 
        // así que acá también lo dejamos con el signo de pregunta (?)
        public int? CodPromo { get; set; }

        public short Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}