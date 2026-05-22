using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class Empleado
{
    public int IdEmpleado { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string? Puesto { get; set; }

    public string? Telefono { get; set; }

    public virtual ICollection<Reservacion> Reservacions { get; set; } = new List<Reservacion>();

    public virtual Usuario? Usuario { get; set; }
}
