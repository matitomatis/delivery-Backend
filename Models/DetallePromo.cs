using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; // ¡Este es el que hace funcionar el JsonIgnore!

namespace delivery.Models
{
    [Table("detalle_promos")]
    public class DetallePromo
    {
        public int CodPromo { get; set; }

        [JsonIgnore]
        [ForeignKey("CodPromo")]
        public virtual Promo? Promo { get; set; } // <-- Fijate el "?" acá

        // Clave Foránea hacia Articulo
        public int CodArticulo { get; set; }

        [JsonIgnore]
        [ForeignKey("CodArticulo")]
        public virtual Articulo? Articulo { get; set; } // <-- Fijate el "?" acá

        [Required]
        public short Cantidad { get; set; }
    }
}