using Microsoft.AspNetCore.Mvc;
using HotelMVC.Models;
using System.Text;
using System.Text.Json;

namespace HotelMVC.Controllers;

public class EmpleadoController : Controller
{
    private readonly IHttpClientFactory _http;
    private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public EmpleadoController(IHttpClientFactory http) => _http = http;

    private IActionResult RedirigirSiNoHaySesion()
    {
        if (HttpContext.Session.GetString("Username") == null)
            return RedirectToAction("Login", "Home");
        return null!;
    }

    public async Task<IActionResult> Index()
    {
        var redir = RedirigirSiNoHaySesion(); if (redir != null) return redir;

        var client = _http.CreateClient("HotelAPI");
        var res = await client.GetAsync("empleado");
        var lista = new List<Empleado>();
        if (res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync();
            lista = JsonSerializer.Deserialize<List<Empleado>>(body, _json) ?? new();
        }
        return View(lista);
    }

    public IActionResult Crear()
    {
        var redir = RedirigirSiNoHaySesion(); if (redir != null) return redir;
        return View(new Empleado());
    }

    [HttpPost]
    public async Task<IActionResult> Crear(Empleado empleado)
    {
        if (!ModelState.IsValid) return View(empleado);

        var client = _http.CreateClient("HotelAPI");
        var json = JsonSerializer.Serialize(empleado);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await client.PostAsync("empleado", content);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Editar(int id)
    {
        var redir = RedirigirSiNoHaySesion(); if (redir != null) return redir;

        var client = _http.CreateClient("HotelAPI");
        var res = await client.GetAsync("empleado");
        var lista = new List<Empleado>();
        if (res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync();
            lista = JsonSerializer.Deserialize<List<Empleado>>(body, _json) ?? new();
        }
        var empleado = lista.FirstOrDefault(e => e.IdEmpleado == id);
        if (empleado == null) return NotFound();
        return View(empleado);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(int id, Empleado empleado)
    {
        if (!ModelState.IsValid) return View(empleado);

        var client = _http.CreateClient("HotelAPI");
        var json = JsonSerializer.Serialize(empleado);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await client.PutAsync($"empleado/{id}", content);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar(int id)
    {
        var client = _http.CreateClient("HotelAPI");
        await client.DeleteAsync($"empleado/{id}");
        return RedirectToAction("Index");
    }
}
