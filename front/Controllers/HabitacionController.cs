using Microsoft.AspNetCore.Mvc;
using HotelMVC.Models;
using System.Text;
using System.Text.Json;

namespace HotelMVC.Controllers;

public class HabitacionController : Controller
{
    private readonly IHttpClientFactory _http;
    private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public HabitacionController(IHttpClientFactory http) => _http = http;

    private IActionResult RedirigirSiNoHaySesion()
    {
        if (HttpContext.Session.GetString("Username") == null)
            return RedirectToAction("Login", "Home");
        return null!;
    }

    private async Task CargarSelectsAsync()
    {
        var client = _http.CreateClient("HotelAPI");

        var resHoteles = await client.GetAsync("hotel");
        var hoteles = new List<Hotel>();
        if (resHoteles.IsSuccessStatusCode)
        {
            var body = await resHoteles.Content.ReadAsStringAsync();
            hoteles = JsonSerializer.Deserialize<List<Hotel>>(body, _json) ?? new();
        }

        var resTipos = await client.GetAsync("tipohabitacion");
        var tipos = new List<TipoHabitacion>();
        if (resTipos.IsSuccessStatusCode)
        {
            var body = await resTipos.Content.ReadAsStringAsync();
            tipos = JsonSerializer.Deserialize<List<TipoHabitacion>>(body, _json) ?? new();
        }

        ViewBag.Hoteles = hoteles;
        ViewBag.Tipos = tipos;
    }

    public async Task<IActionResult> Index()
    {
        var redir = RedirigirSiNoHaySesion(); if (redir != null) return redir;

        var client = _http.CreateClient("HotelAPI");
        var res = await client.GetAsync("habitacion");
        var lista = new List<Habitacion>();
        if (res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync();
            lista = JsonSerializer.Deserialize<List<Habitacion>>(body, _json) ?? new();
        }

        // Cargar hoteles y tipos para mostrar nombres en la tabla
        var resHoteles = await client.GetAsync("hotel");
        var hoteles = new List<Hotel>();
        if (resHoteles.IsSuccessStatusCode)
        {
            var body = await resHoteles.Content.ReadAsStringAsync();
            hoteles = JsonSerializer.Deserialize<List<Hotel>>(body, _json) ?? new();
        }

        var resTipos = await client.GetAsync("tipohabitacion");
        var tipos = new List<TipoHabitacion>();
        if (resTipos.IsSuccessStatusCode)
        {
            var body = await resTipos.Content.ReadAsStringAsync();
            tipos = JsonSerializer.Deserialize<List<TipoHabitacion>>(body, _json) ?? new();
        }

        ViewBag.Hoteles = hoteles;
        ViewBag.Tipos = tipos;
        return View(lista);
    }

    public async Task<IActionResult> Crear()
    {
        var redir = RedirigirSiNoHaySesion(); if (redir != null) return redir;
        await CargarSelectsAsync();
        return View(new Habitacion());
    }

    [HttpPost]
    public async Task<IActionResult> Crear(Habitacion habitacion)
    {
        if (!ModelState.IsValid) { await CargarSelectsAsync(); return View(habitacion); }

        var client = _http.CreateClient("HotelAPI");
        var json = JsonSerializer.Serialize(habitacion);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await client.PostAsync("habitacion", content);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Editar(int id)
    {
        var redir = RedirigirSiNoHaySesion(); if (redir != null) return redir;

        var client = _http.CreateClient("HotelAPI");
        var res = await client.GetAsync("habitacion");
        var lista = new List<Habitacion>();
        if (res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync();
            lista = JsonSerializer.Deserialize<List<Habitacion>>(body, _json) ?? new();
        }
        var habitacion = lista.FirstOrDefault(h => h.IdHabitacion == id);
        if (habitacion == null) return NotFound();

        await CargarSelectsAsync();
        return View(habitacion);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(int id, Habitacion habitacion)
    {
        if (!ModelState.IsValid) { await CargarSelectsAsync(); return View(habitacion); }

        var client = _http.CreateClient("HotelAPI");
        var json = JsonSerializer.Serialize(habitacion);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await client.PutAsync($"habitacion/{id}", content);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar(int id)
    {
        var client = _http.CreateClient("HotelAPI");
        await client.DeleteAsync($"habitacion/{id}");
        return RedirectToAction("Index");
    }
}
