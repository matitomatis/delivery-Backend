using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace delivery.Models
{
    [Table("clientes")]
    public class Cliente
    {
        [Key]
        public int CodCliente { get; set; }

        [Required]
        public string NombreApellido { get; set; }

        [Required]
        public long Telefono { get; set; }

        // La fecha de alta la dejamos como DateTime y le podemos dar un valor por defecto
        public DateTime FechaAlta { get; set; } = DateTime.Now;
    }
}
