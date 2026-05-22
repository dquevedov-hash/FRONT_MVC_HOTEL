using Microsoft.AspNetCore.Mvc;
using HotelAPI.Models;
using System.Collections.Generic;
using System.Linq;
using HotelAPI.Data;

namespace HotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public HotelController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: api/hotel
        [HttpGet]
        public ActionResult<IEnumerable<Hotel>> GetHoteles()
        {
            return _context.Hotels.ToList();
        }

        // POST: api/hotel
        [HttpPost]
        public ActionResult<Hotel> CrearHotel(Hotel hotel)
        {
            _context.Hotels.Add(hotel);
            _context.SaveChanges();
            return Ok(hotel);
        }

        // PUT: api/hotel/1
        [HttpPut("{id}")]
        public ActionResult ActualizarHotel(int id, Hotel hotel)
        {
            var hotelExistente = _context.Hotels.Find(id);

            if (hotelExistente == null)
            {
                return NotFound();
            }

            hotelExistente.Nombre = hotel.Nombre;
            hotelExistente.Direccion = hotel.Direccion;
            hotelExistente.Telefono = hotel.Telefono;

            _context.SaveChanges();

            return Ok(hotelExistente);
        }

        // DELETE: api/hotel/1
        [HttpDelete("{id}")]
        public ActionResult EliminarHotel(int id)
        {
            var hotel = _context.Hotels.Find(id);

            if (hotel == null)
            {
                return NotFound();
            }

            _context.Hotels.Remove(hotel);
            _context.SaveChanges();

            return Ok("Hotel eliminado correctamente");
        }
    }
}