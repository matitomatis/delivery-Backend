using System;
using System.Collections.Generic;

namespace delivery.Models;

public partial class ImagenesArticulo
{
    public int CodImagen { get; set; }

    public string Url { get; set; } = null!;

    public int CodArticulo { get; set; }

    public virtual Articulo CodArticuloNavigation { get; set; } = null!;
}
