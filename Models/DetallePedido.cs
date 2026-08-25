using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace delivery.Models
{
    [Table("detalles_pedido")]
    public class DetallePedido
    {
        public int NroPedido { get; set; }
        [ForeignKey("NroPedido")]
        public virtual Pedido Pedido { get; set; }

        // Clave Foránea y parte de la PK compuesta hacia Promo
        public int CodPromo { get; set; }
        [ForeignKey("CodPromo")]
        public virtual Promo Promo { get; set; }
        [Required]
        public short Cantidad { get; set; }
        [Required]
        public decimal PreUnitario { get; set; }
    }
}
