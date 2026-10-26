using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ZapatillasProject.Models;

public partial class Ventum
{
    public int IdVenta { get; set; }

    public int NumeroVenta { get; set; }

    public string TipoPago { get; set; } = null!;

    public int Monto { get; set; }

    public DateOnly Fecha { get; set; }

    public int? IdUser { get; set; }

    public virtual ICollection<DetalleVentum> DetalleVenta { get; set; } = new List<DetalleVentum>();
    [JsonIgnore]
    public virtual Usuario? IdUserNavigation { get; set; }
}
