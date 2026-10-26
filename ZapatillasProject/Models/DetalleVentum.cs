using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ZapatillasProject.Models;

public partial class DetalleVentum
{
    public int IdDetalle { get; set; }

    public int IdZapatilla { get; set; }

    public int Precio { get; set; }

    public int IdVenta { get; set; }
    [JsonIgnore]
    public virtual Zapatilla IdZapatillaNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Ventum IdVentaNavigation { get; set; } = null!;
}
