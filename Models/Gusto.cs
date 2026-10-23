using System;
using System.Collections.Generic;

namespace delivery.Models;

public partial class Gusto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool HayStock { get; set; }
}
