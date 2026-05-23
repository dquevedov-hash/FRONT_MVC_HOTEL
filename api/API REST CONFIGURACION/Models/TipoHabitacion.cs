using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class TipoHabitacion
{
    public int IdTipo { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public virtual ICollection<Habitacion> Habitacions { get; set; } = new List<Habitacion>();
}
