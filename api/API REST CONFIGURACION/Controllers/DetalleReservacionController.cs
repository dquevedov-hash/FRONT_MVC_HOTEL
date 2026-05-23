using Microsoft.AspNetCore.Mvc;
using HotelAPI.Models;
using HotelAPI.Data;
using System.Collections.Generic;
using System.Linq;

namespace HotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DetalleReservacionController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public DetalleReservacionController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<DetalleReservacion>> GetDetalles()
        {
            return _context.DetalleReservacions.ToList();
        }

        [HttpPost]
        public ActionResult<DetalleReservacion> CrearDetalle(DetalleReservacion detalle)
        {
            _context.DetalleReservacions.Add(detalle);
            _context.SaveChanges();
            return Ok(detalle);
        }

        [HttpPut("{id}")]
        public ActionResult ActualizarDetalle(int id, DetalleReservacion detalle)
        {
            var detalleExistente = _context.DetalleReservacions.Find(id);
            if (detalleExistente == null) return NotFound();

            detalleExistente.IdReservacion = detalle.IdReservacion;
            detalleExistente.IdHabitacion = detalle.IdHabitacion;
            detalleExistente.Dias = detalle.Dias;
            detalleExistente.Subtotal = detalle.Subtotal;

            _context.SaveChanges();
            return Ok(detalleExistente);
        }

        [HttpDelete("{id}")]
        public ActionResult EliminarDetalle(int id)
        {
            var detalle = _context.DetalleReservacions.Find(id);
            if (detalle == null) return NotFound();

            _context.DetalleReservacions.Remove(detalle);
            _context.SaveChanges();
            return Ok("Detalle de reservación eliminado correctamente");
        }
    }
}