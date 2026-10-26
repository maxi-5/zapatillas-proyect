using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ZapatillasProject.Models;

public partial class Usuario
{
    public int IdUser { get; set; }

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;
    [JsonIgnore]
    public string Password { get; set; } = null!;

    public int IdRol { get; set; }

    public virtual Rol? IdRolNavigation { get; set; } = null;
    [JsonIgnore]
    public virtual ICollection<Resena> Resenas { get; set; } = new List<Resena>();
    [JsonIgnore]
    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
