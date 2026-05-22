using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? IdEmpleado { get; set; }

    public string? Rol { get; set; }  // Linea para verificar el rol

    public virtual Empleado? IdEmpleadoNavigation { get; set; }
}