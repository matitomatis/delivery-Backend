using System;
using System.Collections.Generic;

namespace delivery.Models;

public partial class DetallePedido
{
    public int CodDetalle { get; set; }

    public int CodPedido { get; set; }

    public int? CodArticulo { get; set; }

    public int? CodPromo { get; set; }

    public short Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public virtual Articulo? CodArticuloNavigation { get; set; }

    public virtual Pedido CodPedidoNavigation { get; set; } = null!;

    public virtual Promo? CodPromoNavigation { get; set; }
}
