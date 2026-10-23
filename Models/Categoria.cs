using System;
using System.Collections.Generic;

namespace delivery.Models;

public partial class Categoria
{
    public int CodCategoria { get; set; }

    public string Nombre { get; set; } = null!;

    public int? BannerId { get; set; }

    public int CodRubro { get; set; }

    public virtual ICollection<Articulo> Articulos { get; set; } = new List<Articulo>();

    public virtual Banner? Banner { get; set; }

    public virtual Rubro CodRubroNavigation { get; set; } = null!;
}
