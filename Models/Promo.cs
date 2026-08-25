using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace delivery.Models
{
    [Table("promos")]
    public class Promo
    {
        [Key]
        public int CodPromo { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; } // Puede ser nulo

        [Required]
        public string Categoria { get; set; }

        public string UrlImagen { get; set; } // Puede ser nulo

        [Required]
        public decimal PrecioVenta { get; set; }

        // Mapea perfecto con el "bit" de SQL Server
        public bool Activa { get; set; } = true;

        // Propiedad de navegación: Una promo tiene una lista de detalles asociados
        public virtual ICollection<DetallePromo> Detalles { get; set; }
    }
}
