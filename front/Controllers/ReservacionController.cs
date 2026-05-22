using Microsoft.AspNetCore.Mvc;
using HotelMVC.Models;
using System.Text;
using System.Text.Json;

namespace HotelMVC.Controllers;

public class ReservacionController : Controller
{
    private readonly IHttpClientFactory _http;
    private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public ReservacionController(IHttpClientFactory http) => _http = http;

    private IActionResult RedirigirSiNoHaySesion()
    {
        if (HttpContext.Session.GetString("Username") == null)
            return RedirectToAction("Login", "Home");
        return null!;
    }

    private async Task CargarSelectsAsync()
    {
        var client = _http.CreateClient("HotelAPI");

        var resClientes = await client.GetAsync("cliente");
        var clientes = new List<Cliente>();
        if (resClientes.IsSuccessStatusCode)
        {
            var body = await resClientes.Content.ReadAsStringAsync();
            clientes = JsonSerializer.Deserialize<List<Cliente>>(body, _json) ?? new();
        }

        var resEmpleados = await client.GetAsync("empleado");
        var empleados = new List<Empleado>();
        if (resEmpleados.IsSuccessStatusCode)
        {
            var body = await resEmpleados.Content.ReadAsStringAsync();
            empleados = JsonSerializer.Deserialize<List<Empleado>>(body, _json) ?? new();
        }

        var resHoteles = await client.GetAsync("hotel");
        var hoteles = new List<Hotel>();
        if (resHoteles.IsSuccessStatusCode)
        {
            var body = await resHoteles.Content.ReadAsStringAsync();
            hoteles = JsonSerializer.Deserialize<List<Hotel>>(body, _json) ?? new();
        }

        ViewBag.Clientes = clientes;
        ViewBag.Empleados = empleados;
        ViewBag.Hoteles = hoteles;
    }

    public async Task<IActionResult> Index()
    {
        var redir = RedirigirSiNoHaySesion(); if (redir != null) return redir;

        var client = _http.CreateClient("HotelAPI");
        var res = await client.GetAsync("reservacion");
        var lista = new List<Reservacion>();
        if (res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync();
            lista = JsonSerializer.Deserialize<List<Reservacion>>(body, _json) ?? new();
        }

        await CargarSelectsAsync();
        return View(lista);
    }

    public async Task<IActionResult> Crear()
    {
        var redir = RedirigirSiNoHaySesion(); if (redir != null) return redir;
        await CargarSelectsAsync();
        return View(new Reservacion { FechaInicio = DateOnly.FromDateTime(DateTime.Today), FechaFin = DateOnly.FromDateTime(DateTime.Today.AddDays(1)) });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(Reservacion reservacion)
    {
        if (!ModelState.IsValid) { await CargarSelectsAsync(); return View(reservacion); }

        var client = _http.CreateClient("HotelAPI");
        var json = JsonSerializer.Serialize(reservacion);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await client.PostAsync("reservacion", content);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Editar(int id)
    {
        var redir = RedirigirSiNoHaySesion(); if (redir != null) return redir;

        var client = _http.CreateClient("HotelAPI");
        var res = await client.GetAsync("reservacion");
        var lista = new List<Reservacion>();
        if (res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync();
            lista = JsonSerializer.Deserialize<List<Reservacion>>(body, _json) ?? new();
        }
        var reservacion = lista.FirstOrDefault(r => r.IdReservacion == id);
        if (reservacion == null) return NotFound();

        await CargarSelectsAsync();
        return View(reservacion);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(int id, Reservacion reservacion)
    {
        if (!ModelState.IsValid) { await CargarSelectsAsync(); return View(reservacion); }

        var client = _http.CreateClient("HotelAPI");
        var json = JsonSerializer.Serialize(reservacion);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await client.PutAsync($"reservacion/{id}", content);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar(int id)
    {
        var client = _http.CreateClient("HotelAPI");
        await client.DeleteAsync($"reservacion/{id}");
        return RedirectToAction("Index");
    }
}
