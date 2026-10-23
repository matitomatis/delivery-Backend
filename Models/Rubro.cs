using System;
using System.Collections.Generic;

namespace delivery.Models;

public partial class Rubro
{
    public int CodRubro { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Categoria> Categoria { get; set; } = new List<Categoria>();
}
