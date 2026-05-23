using Microsoft.AspNetCore.Mvc;
using HotelAPI.Models;
using HotelAPI.Data;
using System.Collections.Generic;
using System.Linq;

namespace HotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HabitacionController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public HabitacionController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Habitacion>> GetHabitaciones()
        {
            return _context.Habitacions.ToList();
        }

        [HttpPost]
        public ActionResult<Habitacion> CrearHabitacion(Habitacion habitacion)
        {
            _context.Habitacions.Add(habitacion);
            _context.SaveChanges();
            return Ok(habitacion);
        }

        [HttpPut("{id}")]
        public ActionResult ActualizarHabitacion(int id, Habitacion habitacion)
        {
            var habitacionExistente = _context.Habitacions.Find(id);
            if (habitacionExistente == null) return NotFound();

            habitacionExistente.Numero = habitacion.Numero;
            habitacionExistente.IdTipo = habitacion.IdTipo;
            habitacionExistente.IdHotel = habitacion.IdHotel;
            habitacionExistente.Estado = habitacion.Estado;

            _context.SaveChanges();
            return Ok(habitacionExistente);
        }

        [HttpDelete("{id}")]
        public ActionResult EliminarHabitacion(int id)
        {
            var habitacion = _context.Habitacions.Find(id);
            if (habitacion == null) return NotFound();

            _context.Habitacions.Remove(habitacion);
            _context.SaveChanges();
            return Ok("Habitación eliminada correctamente");
        }
    }
}