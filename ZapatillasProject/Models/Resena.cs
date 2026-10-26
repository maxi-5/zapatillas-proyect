using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ZapatillasProject.Models;

public partial class Resena
{
    public int IdResena { get; set; }

    public string Resena1 { get; set; } = null!;

    public int Valoracion { get; set; }

    public int IdZapatilla { get; set; }

    public int? IdUser { get; set; }
    [JsonIgnore]
    public virtual Zapatilla IdZapatillaNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Usuario? IdUserNavigation { get; set; }
}
