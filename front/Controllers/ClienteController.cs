using Microsoft.AspNetCore.Mvc;
using HotelMVC.Models;
using System.Text;
using System.Text.Json;

namespace HotelMVC.Controllers;

public class ClienteController : Controller
{
    private readonly IHttpClientFactory _http;
    private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public ClienteController(IHttpClientFactory http) => _http = http;

    private IActionResult RedirigirSiNoHaySesion()
    {
        if (HttpContext.Session.GetString("Username") == null)
            return RedirectToAction("Login", "Home");
        return null!;
    }

    // GET: /Cliente
    public async Task<IActionResult> Index()
    {
        var redir = RedirigirSiNoHaySesion(); if (redir != null) return redir;

        var client = _http.CreateClient("HotelAPI");
        var res = await client.GetAsync("cliente");
        var lista = new List<Cliente>();
        if (res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync();
            lista = JsonSerializer.Deserialize<List<Cliente>>(body, _json) ?? new();
        }
        return View(lista);
    }

    // GET: /Cliente/Crear
    public IActionResult Crear()
    {
        var redir = RedirigirSiNoHaySesion(); if (redir != null) return redir;
        return View(new Cliente());
    }

    // POST: /Cliente/Crear
    [HttpPost]
    public async Task<IActionResult> Crear(Cliente cliente)
    {
        if (!ModelState.IsValid) return View(cliente);

        var client = _http.CreateClient("HotelAPI");
        var json = JsonSerializer.Serialize(cliente);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await client.PostAsync("cliente", content);
        return RedirectToAction("Index");
    }

    // GET: /Cliente/Editar/5
    public async Task<IActionResult> Editar(int id)
    {
        var redir = RedirigirSiNoHaySesion(); if (redir != null) return redir;

        var client = _http.CreateClient("HotelAPI");
        var res = await client.GetAsync("cliente");
        var lista = new List<Cliente>();
        if (res.IsSuccessStatusCode)
        {
            var body = await res.Content.ReadAsStringAsync();
            lista = JsonSerializer.Deserialize<List<Cliente>>(body, _json) ?? new();
        }
        var cliente = lista.FirstOrDefault(c => c.IdCliente == id);
        if (cliente == null) return NotFound();
        return View(cliente);
    }

    // POST: /Cliente/Editar/5
    [HttpPost]
    public async Task<IActionResult> Editar(int id, Cliente cliente)
    {
        if (!ModelState.IsValid) return View(cliente);

        var client = _http.CreateClient("HotelAPI");
        var json = JsonSerializer.Serialize(cliente);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await client.PutAsync($"cliente/{id}", content);
        return RedirectToAction("Index");
    }

    // POST: /Cliente/Eliminar/5
    [HttpPost]
    public async Task<IActionResult> Eliminar(int id)
    {
        var client = _http.CreateClient("HotelAPI");
        await client.DeleteAsync($"cliente/{id}");
        return RedirectToAction("Index");
    }
}
