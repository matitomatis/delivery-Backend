using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace delivery.Models
{
    [Table("pedidos")]
    public class Pedido
    {
        [Key]
        public int Nro_pedido { get; set; }
        public int Cod_cliente { get; set; }
        public DateTime FechaPedido { get; set; } = DateTime.Now;

        
        public string? DireccionEnvio { get; set; }

        [Required]
        public string? FormaEntrega { get; set; }

        [Required]
        public string? FormaPago { get; set; }

        public string? Estado { get; set; } 

        [Required]
        public decimal Total { get; set; }

    }
}
