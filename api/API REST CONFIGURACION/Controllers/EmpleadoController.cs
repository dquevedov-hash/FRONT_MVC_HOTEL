using Microsoft.AspNetCore.Mvc;
using HotelAPI.Models;
using HotelAPI.Data;
using System.Collections.Generic;
using System.Linq;

namespace HotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadoController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public EmpleadoController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Empleado>> GetEmpleados()
        {
            return _context.Empleados.ToList();
        }

        [HttpPost]
        public ActionResult<Empleado> CrearEmpleado(Empleado empleado)
        {
            _context.Empleados.Add(empleado);
            _context.SaveChanges();
            return Ok(empleado);
        }

        [HttpPut("{id}")]
        public ActionResult ActualizarEmpleado(int id, Empleado empleado)
        {
            var empleadoExistente = _context.Empleados.Find(id);
            if (empleadoExistente == null) return NotFound();

            empleadoExistente.Nombre = empleado.Nombre;
            empleadoExistente.Apellido = empleado.Apellido;
            empleadoExistente.Puesto = empleado.Puesto;
            empleadoExistente.Telefono = empleado.Telefono;

            _context.SaveChanges();
            return Ok(empleadoExistente);
        }

        [HttpDelete("{id}")]
        public ActionResult EliminarEmpleado(int id)
        {
            var empleado = _context.Empleados.Find(id);
            if (empleado == null) return NotFound();

            _context.Empleados.Remove(empleado);
            _context.SaveChanges();
            return Ok("Empleado eliminado correctamente");
        }
    }
}