using Microsoft.AspNetCore.Mvc;
using HotelAPI.Models;
using HotelAPI.Data;
using System.Collections.Generic;
using System.Linq;

namespace HotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoHabitacionController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public TipoHabitacionController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TipoHabitacion>> GetTipoHabitaciones()
        {
            return _context.TipoHabitacions.ToList();
        }

        [HttpPost]
        public ActionResult<TipoHabitacion> CrearTipoHabitacion(TipoHabitacion tipoHabitacion)
        {
            _context.TipoHabitacions.Add(tipoHabitacion);
            _context.SaveChanges();
            return Ok(tipoHabitacion);
        }

        [HttpPut("{id}")]
        public ActionResult ActualizarTipoHabitacion(int id, TipoHabitacion tipoHabitacion)
        {
            var tipoExistente = _context.TipoHabitacions.Find(id);
            if (tipoExistente == null) return NotFound();

            tipoExistente.Nombre = tipoHabitacion.Nombre;
            tipoExistente.Descripcion = tipoHabitacion.Descripcion;
            tipoExistente.Precio = tipoHabitacion.Precio;

            _context.SaveChanges();
            return Ok(tipoExistente);
        }

        [HttpDelete("{id}")]
        public ActionResult EliminarTipoHabitacion(int id)
        {
            var tipo = _context.TipoHabitacions.Find(id);
            if (tipo == null) return NotFound();

            _context.TipoHabitacions.Remove(tipo);
            _context.SaveChanges();
            return Ok("Tipo de habitación eliminado correctamente");
        }
    }
}