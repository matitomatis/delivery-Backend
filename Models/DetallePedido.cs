using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace delivery.Models
{
    [Table("detalle_pedidos")]
    public class DetallePedido
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CodDetalle { get; set; }

        public int CodPedido { get; set; }
        [JsonIgnore]
        [ForeignKey("CodPedido")]
        public virtual Pedido? Pedido { get; set; }

        // Puede ser un Artículo...
        public int? CodArticulo { get; set; }
        [JsonIgnore]
        [ForeignKey("CodArticulo")]
        public virtual Articulo? Articulo { get; set; }

        // ... o puede ser una Promo
        public int? CodPromo { get; set; }
        [JsonIgnore]
        [ForeignKey("CodPromo")]
        public virtual Promo? Promo { get; set; }

        [Required]
        public short Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }
    }
}