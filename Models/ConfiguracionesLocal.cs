using System;
using System.Collections.Generic;

namespace delivery.Models;

public partial class ConfiguracionesLocal
{
    public int Id { get; set; }

    public decimal TarifaEnvio { get; set; }

    public string? WhatsApp { get; set; }

    public string? Instagram { get; set; }

    public string? Facebook { get; set; }

    public string? GoogleMaps { get; set; }

    public string Alias { get; set; } = null!;
}
