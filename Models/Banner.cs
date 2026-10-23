using System;
using System.Collections.Generic;

namespace delivery.Models;

public partial class Banner
{
    public int CodBanner { get; set; }

    public string Titulo { get; set; } = null!;

    public string Subtitulo { get; set; } = null!;

    public string BackgroundColor { get; set; } = null!;

    public virtual ICollection<Categoria> Categoria { get; set; } = new List<Categoria>();
}
