using System;
using System.Collections.Generic;

namespace delivery.Models;

public partial class Articulo
{
    public int CodArticulo { get; set; }

    public string Descripcion { get; set; } = null!;

    public decimal Costo { get; set; }

    public int Stock { get; set; }

    public int StockMinimo { get; set; }

    public string UrlImagen { get; set; } = null!;

    public int CategoriaId { get; set; }

    public int CodRubro { get; set; }

    public int MaxGustos { get; set; }

    public virtual Categoria Categoria { get; set; } = null!;

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    public virtual ICollection<DetallePromo> DetallePromos { get; set; } = new List<DetallePromo>();

    public virtual ICollection<ImagenesArticulo> ImagenesArticulos { get; set; } = new List<ImagenesArticulo>();
}
