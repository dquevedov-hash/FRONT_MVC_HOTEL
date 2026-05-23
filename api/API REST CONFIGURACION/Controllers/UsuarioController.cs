using Microsoft.AspNetCore.Mvc;
using HotelAPI.Models;
using HotelAPI.Data;
using System.Collections.Generic;
using System.Linq;

namespace HotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public UsuarioController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: api/usuario
        [HttpGet]
        public ActionResult<IEnumerable<Usuario>> GetUsuarios()
        {
            return _context.Usuarios.ToList();
        }

        // POST: api/usuario
        [HttpPost]
        public ActionResult<Usuario> CrearUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
            return Ok(usuario);
        }

        // POST: api/usuario/login
        // Este es el método que cumple con la autenticación requerida
        [HttpPost("login")]
        public ActionResult Login([FromBody] Usuario loginRequest)
        {
            // Busca en la base de datos si existe alguien con ese usuario y contraseña exactos
            var usuarioValido = _context.Usuarios
                .Where(u => u.Username == loginRequest.Username && u.Password == loginRequest.Password)
                .FirstOrDefault();

            if (usuarioValido == null)
            {
                // Devuelve un error 401 Unauthorized si las credenciales son incorrectas
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            // Devuelve los datos del usuario con un código 200 OK si todo está correcto
            return Ok(usuarioValido);
        }
    }
}