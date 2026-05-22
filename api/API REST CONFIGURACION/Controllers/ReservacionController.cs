using Microsoft.AspNetCore.Mvc;
using HotelAPI.Models;
using HotelAPI.Data;
using System.Collections.Generic;
using System.Linq;

namespace HotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservacionController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public ReservacionController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Reservacion>> GetReservaciones()
        {
            return _context.Reservacions.ToList();
        }

        [HttpPost]
        public ActionResult<Reservacion> CrearReservacion(Reservacion reservacion)
        {
            _context.Reservacions.Add(reservacion);
            _context.SaveChanges();
            return Ok(reservacion);
        }

        [HttpPut("{id}")]
        public ActionResult ActualizarReservacion(int id, Reservacion reservacion)
        {
            var reservacionExistente = _context.Reservacions.Find(id);
            if (reservacionExistente == null) return NotFound();

            reservacionExistente.IdCliente = reservacion.IdCliente;
            reservacionExistente.IdEmpleado = reservacion.IdEmpleado;
            reservacionExistente.IdHotel = reservacion.IdHotel;
            reservacionExistente.FechaInicio = reservacion.FechaInicio;
            reservacionExistente.FechaFin = reservacion.FechaFin;
            reservacionExistente.Total = reservacion.Total;

            _context.SaveChanges();
            return Ok(reservacionExistente);
        }

        [HttpDelete("{id}")]
        public ActionResult EliminarReservacion(int id)
        {
            var reservacion = _context.Reservacions.Find(id);
            if (reservacion == null) return NotFound();

            _context.Reservacions.Remove(reservacion);
            _context.SaveChanges();
            return Ok("Reservación eliminada correctamente");
        }
    }
}