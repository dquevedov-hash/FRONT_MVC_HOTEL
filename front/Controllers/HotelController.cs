using Microsoft.AspNetCore.Mvc;
using HotelMVC.Models;
using System.Text;
using System.Text.Json;

namespace HotelMVC.Controllers;

public class HotelController : Controller
{
    private readonly IHttpClientFactory _http;
    private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public HotelController(IHttpClientFactory http) => _http = http;

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
        var res = await client.GetAsync("hotel");
        var lista = new List<Hotel>();
        if (res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync();
            lista = JsonSerializer.Deserialize<List<Hotel>>(body, _json) ?? new();
        }
        return View(lista);
    }

    public IActionResult Crear()
    {
        var redir = RedirigirSiNoHaySesion(); if (redir != null) return redir;
        return View(new Hotel());
    }

    [HttpPost]
    public async Task<IActionResult> Crear(Hotel hotel)
    {
        if (!ModelState.IsValid) return View(hotel);

        var client = _http.CreateClient("HotelAPI");
        var json = JsonSerializer.Serialize(hotel);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await client.PostAsync("hotel", content);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Editar(int id)
    {
        var redir = RedirigirSiNoHaySesion(); if (redir != null) return redir;

        var client = _http.CreateClient("HotelAPI");
        var res = await client.GetAsync("hotel");
        var lista = new List<Hotel>();
        if (res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync();
            lista = JsonSerializer.Deserialize<List<Hotel>>(body, _json) ?? new();
        }
        var hotel = lista.FirstOrDefault(h => h.IdHotel == id);
        if (hotel == null) return NotFound();
        return View(hotel);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(int id, Hotel hotel)
    {
        if (!ModelState.IsValid) return View(hotel);

        var client = _http.CreateClient("HotelAPI");
        var json = JsonSerializer.Serialize(hotel);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await client.PutAsync($"hotel/{id}", content);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar(int id)
    {
        var client = _http.CreateClient("HotelAPI");
        await client.DeleteAsync($"hotel/{id}");
        return RedirectToAction("Index");
    }
}
