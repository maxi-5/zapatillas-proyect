using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ZapatillasProject.Models;

public partial class Marca
{
    public int IdMarca { get; set; }

    public string Nombre { get; set; } = null!;

    public string Pais { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Zapatilla> Zapatillas { get; set; } = new List<Zapatilla>();
}
