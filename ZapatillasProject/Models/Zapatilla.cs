using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ZapatillasProject.Models;

public partial class Zapatilla
{
    public int IdZapatilla { get; set; }

    public string Modelo { get; set; } = null!;

    public int Precio { get; set; }

    public string? Foto { get; set; }

    public int IdCategoria { get; set; }

    public int IdMarca { get; set; }

    public string? Descripcion { get; set; }
    [JsonIgnore]
    public virtual ICollection<DetalleVentum> DetalleVenta { get; set; } = new List<DetalleVentum>();
    [JsonIgnore]
    public virtual Categorium IdCategoriaNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Marca IdMarcaNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Resena> Resenas { get; set; } = new List<Resena>();
    [JsonIgnore]
    public virtual ICollection<StockTalla> StockTallas { get; set; } = new List<StockTalla>();
}
