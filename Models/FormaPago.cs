using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace delivery.Models
{
    [Table("formas_pago")]
    public class FormaPago
    {
        [Key]
        public int CodFormaPago { get; set; }

        [Required]
        [MaxLength(50)]
        public string Descripcion { get; set; } // Ej: Efectivo, Transferencia, Tarjeta
    }
}
