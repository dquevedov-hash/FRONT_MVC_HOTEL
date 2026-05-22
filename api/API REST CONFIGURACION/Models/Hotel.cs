using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class Hotel
{
    public int IdHotel { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public virtual ICollection<Habitacion> Habitacions { get; set; } = new List<Habitacion>();

    public virtual ICollection<Reservacion> Reservacions { get; set; } = new List<Reservacion>();
}
