using System;
using System.Collections.Generic;

namespace delivery.Models;

public partial class Promo
{
    public int CodPromo { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public string UrlImagen { get; set; } = null!;

    public decimal PrecioVenta { get; set; }

    public bool Activa { get; set; }

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    public virtual ICollection<DetallePromo> DetallePromos { get; set; } = new List<DetallePromo>();
}
