namespace HotelMVC.Models;

public class Cliente
{
    public int IdCliente { get; set; }
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Dpi { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Correo { get; set; } = "";
}

public class Hotel
{
    public int IdHotel { get; set; }
    public string Nombre { get; set; } = "";
    public string Direccion { get; set; } = "";
    public string Telefono { get; set; } = "";
}

public class TipoHabitacion
{
    public int IdTipo { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public decimal Precio { get; set; }
}

public class Habitacion
{
    public int IdHabitacion { get; set; }
    public string Numero { get; set; } = "";
    public int IdTipo { get; set; }
    public int IdHotel { get; set; }
    public string Estado { get; set; } = "";
}

public class Empleado
{
    public int IdEmpleado { get; set; }
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Puesto { get; set; } = "";
    public string Telefono { get; set; } = "";
}

public class Reservacion
{
    public int IdReservacion { get; set; }
    public int IdCliente { get; set; }
    public int IdEmpleado { get; set; }
    public int IdHotel { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public decimal? Total { get; set; }
}

public class Usuario
{
    public int IdUsuario { get; set; }
    public int IdEmpleado { get; set; }
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Rol { get; set; } = "";
}

public class LoginRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}
