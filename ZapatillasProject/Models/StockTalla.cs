using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ZapatillasProject.Models;

public partial class StockTalla
{
    public int IdStock { get; set; }

    public int IdZapatilla { get; set; }

    public decimal Talla { get; set; }

    public int Cantidad { get; set; }
    [JsonIgnore]
    public virtual Zapatilla IdZapatillaNavigation { get; set; } = null!;
}
