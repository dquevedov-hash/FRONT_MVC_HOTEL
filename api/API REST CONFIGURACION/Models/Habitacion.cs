using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class Habitacion
{
    public int IdHabitacion { get; set; }

    public string Numero { get; set; } = null!;

    public int IdTipo { get; set; }

    public int IdHotel { get; set; }

    public string Estado { get; set; } = null!;

    public virtual ICollection<DetalleReservacion> DetalleReservacions { get; set; } = new List<DetalleReservacion>();

    // ← El ? cambio para que funcione el front
    public virtual Hotel? IdHotelNavigation { get; set; }

    public virtual TipoHabitacion? IdTipoNavigation { get; set; }
}