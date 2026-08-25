using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace delivery.Models
{
    [Table("articulos")]
    public class Articulo
    {
        [Key] // Especifica que este campo es la clave primaria
        public int CodArticulo { get; set; }

        [Required] // Indica que la descripción no puede quedar en blanco (NOT NULL)
        public string Descripcion { get; set; }

        [Required]
        public decimal Costo { get; set; }

        [Required]
        public short Stock { get; set; }

        // Como el stock mínimo permitía nulos en nuestra base de datos, 
        // le ponemos el signo de interrogación "?" al tipo de dato.
        public short? StockMinimo { get; set; }
    
    }
}
