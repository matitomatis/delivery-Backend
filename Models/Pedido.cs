using System;
using System.Collections.Generic;

namespace delivery.Models;

public partial class Pedido
{
    public int CodPedido { get; set; }

    public DateTime Fecha { get; set; }

    public int CodCliente { get; set; }

    public int CodFormaPago { get; set; }

    public int CodTipoEnvio { get; set; }

    public decimal Total { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Cliente CodClienteNavigation { get; set; } = null!;

    public virtual FormaPago CodFormaPagoNavigation { get; set; } = null!;

    public virtual TipoEnvio CodTipoEnvioNavigation { get; set; } = null!;

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();
}
