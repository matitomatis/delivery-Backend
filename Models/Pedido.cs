using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace delivery.Models
{
    [Table("pedidos")]
    public class Pedido
    {
        [Key]
        public int CodPedido { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        // --- RELACIONES ---
        public int CodCliente { get; set; }
        [JsonIgnore]
        [ForeignKey("CodCliente")]
        public virtual Cliente? Cliente { get; set; }

        public int CodFormaPago { get; set; }
        [JsonIgnore]
        [ForeignKey("CodFormaPago")]
        public virtual FormaPago? FormaPago { get; set; }

        public int CodTipoEnvio { get; set; }
        [JsonIgnore]
        [ForeignKey("CodTipoEnvio")]
        public virtual TipoEnvio? TipoEnvio { get; set; }

        public decimal Total { get; set; }

        // La lista de cosas que pidió
        public List<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }
}