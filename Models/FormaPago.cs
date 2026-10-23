using System;
using System.Collections.Generic;

namespace delivery.Models;

public partial class FormaPago
{
    public int CodFormaPago { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
