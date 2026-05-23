using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class DetalleReservacion
{
    public int IdDetalle { get; set; }

    public int IdReservacion { get; set; }

    public int IdHabitacion { get; set; }

    public int Dias { get; set; }

    public decimal? Subtotal { get; set; }

    public virtual Habitacion IdHabitacionNavigation { get; set; } = null!;

    public virtual Reservacion IdReservacionNavigation { get; set; } = null!;
}
