using Microsoft.AspNetCore.Mvc;
using HotelMVC.Models;
using System.Text;
using System.Text.Json;

namespace HotelMVC.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    public HomeController(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    // Pasa la URL base de la API a todas las vistas del dashboard
    private void SetApiBase()
    {
        ViewBag.ApiBase = _config["ApiSettings:BaseUrl"] ?? "http://localhost:5000/api";
    }

    // GET: / → redirige a login si no hay sesión
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("Username") == null)
            return RedirectToAction("Login");

        SetApiBase();
        return View();
    }

    // GET: /Home/Login
    public IActionResult Login()
    {
        if (HttpContext.Session.GetString("Username") != null)
            return RedirectToAction("Index");

        return View();
    }

    // POST: /Home/Login
    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var client = _httpClientFactory.CreateClient("HotelAPI");
        var json = JsonSerializer.Serialize(new
        {
            username = request.Username,
            password = request.Password
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync("usuario/login", content);
            if (response.IsSuccessStatusCode)
            {
                var body    = await response.Content.ReadAsStringAsync();
                var usuario = JsonSerializer.Deserialize<Usuario>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                HttpContext.Session.SetString("Username", usuario!.Username);
                HttpContext.Session.SetString("Rol",      usuario.Rol);
                HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);

                return RedirectToAction("Index");
            }
        }
        catch { }

        ViewBag.Error = "Usuario o contraseña incorrectos.";
        return View(request);
    }

    // GET: /Home/Logout
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
