using Microsoft.AspNetCore.Mvc;
using HotelAPI.Models;
using HotelAPI.Data;
using System.Collections.Generic;
using System.Linq;

namespace HotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public ClienteController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: api/cliente
        [HttpGet]
        public ActionResult<IEnumerable<Cliente>> GetClientes()
        {
            return _context.Clientes.ToList();
        }
        // POST: api/cliente/1
        [HttpPost]
        public ActionResult<Cliente> CrearCliente(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            _context.SaveChanges();
            return Ok(cliente);
        }

        // PUT: api/cliente

        [HttpPut("{id}")]
        public ActionResult ActualizarCliente(int id, Cliente cliente)
        {
            var clienteExistente = _context.Clientes.Find(id);

            if (clienteExistente == null)
            {
                return NotFound();
            }

            clienteExistente.Nombre = cliente.Nombre;
            clienteExistente.Apellido = cliente.Apellido;
            clienteExistente.Dpi = cliente.Dpi;
            clienteExistente.Telefono = cliente.Telefono;
            clienteExistente.Correo = cliente.Correo;

            _context.SaveChanges();

            return Ok(clienteExistente);
        }



        // DELETE: api/cliente/1
        [HttpDelete("{id}")]
        public ActionResult EliminarCliente(int id)
        {
            var cliente = _context.Clientes.Find(id);

            if (cliente == null)
            {
                return NotFound();
            }

            _context.Clientes.Remove(cliente);
            _context.SaveChanges();

            return Ok("Cliente eliminado");
        }
}
}