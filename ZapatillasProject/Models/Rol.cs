using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ZapatillasProject.Models;

public partial class Rol
{
    public int IdRol { get; set; }

    public string Nombre { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
