using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace delivery.Models
{
    [Table("tipos_envio")]
    public class TipoEnvio
    {
        [Key]
        public int CodTipoEnvio { get; set; }

        [Required]
        [MaxLength(50)]
        public string Descripcion { get; set; } // Ej: Delivery, Retiro en local
    }
}
