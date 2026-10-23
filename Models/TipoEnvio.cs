using System;
using System.Collections.Generic;

namespace delivery.Models;

public partial class TipoEnvio
{
    public int CodTipoEnvio { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
