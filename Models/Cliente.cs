using System;
using System.Collections.Generic;

namespace delivery.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Cuit { get; set; }

    public string? NombreLocal { get; set; }

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
