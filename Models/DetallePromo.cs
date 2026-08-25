using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace delivery.Models
{
    [Table("detalle_promos")]
    public class DetallePromo
    {
        public int CodPromo { get; set; }
        [ForeignKey("CodPromo")]
        public virtual Promo Promo { get; set; }

        // Clave Foránea hacia Articulo
        public int CodArticulo { get; set; }
        [ForeignKey("CodArticulo")]
        public virtual Articulo Articulo { get; set; }

        [Required]
        public short Cantidad { get; set; }
    }
}
