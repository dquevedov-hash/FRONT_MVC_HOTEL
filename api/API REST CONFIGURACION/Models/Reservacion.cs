using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class Reservacion
{
    public int IdReservacion { get; set; }

    public int IdCliente { get; set; }

    public int IdEmpleado { get; set; }

    public int IdHotel { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public decimal? Total { get; set; }

    public virtual ICollection<DetalleReservacion> DetalleReservacions { get; set; } = new List<DetalleReservacion>();

    // ← El ? cambio para que funcione el front 
    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual Empleado? IdEmpleadoNavigation { get; set; }

    public virtual Hotel? IdHotelNavigation { get; set; }
}