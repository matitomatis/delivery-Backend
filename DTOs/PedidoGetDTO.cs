using System;
using System.Collections.Generic;

namespace delivery.DTOs
{
    // Este es el formato limpio que la API le va a devolver a la pantalla/frontend
    public class PedidoGetDTO
    {
        public int CodPedido { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public int CodCliente { get; set; }
        public int CodFormaPago { get; set; }
        public int CodTipoEnvio { get; set; }

        public List<DetallePedidoGetDTO> Detalles { get; set; } = new List<DetallePedidoGetDTO>();
    }

    public class DetallePedidoGetDTO
    {
        public int CodArticulo { get; set; }
        public short Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario; // ¡Un extra! Calculamos el subtotal al vuelo para la pantalla
    }
}