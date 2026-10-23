using System;
using System.Collections.Generic;

namespace delivery.Models;

public partial class DetallePromo
{
    public int CodPromo { get; set; }

    public int CodArticulo { get; set; }

    public short Cantidad { get; set; }

    public virtual Articulo CodArticuloNavigation { get; set; } = null!;

    public virtual Promo CodPromoNavigation { get; set; } = null!;
}
