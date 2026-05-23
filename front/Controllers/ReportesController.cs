using Microsoft.AspNetCore.Mvc;
using HotelMVC.Models;
using System.Text.Json;

namespace HotelMVC.Controllers;

/// <summary>
/// Controlador MVC de Reportes.
/// Rutas disponibles según rol:
///   Administrador → todos los reportes
///   Supervisor    → ocupación, ingresos, reservaciones, clientes
///   Recepcionista → sin acceso (redirige a Home)
/// </summary>
public class ReportesController : Controller
{
    private readonly IHttpClientFactory _http;
    private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public ReportesController(IHttpClientFactory http) => _http = http;

    // ── Protección de sesión y rol ──────────────────────────────────────────

    private IActionResult? RedirigirSiNoHaySesion()
    {
        if (HttpContext.Session.GetString("Username") == null)
            return RedirectToAction("Login", "Home");
        return null;
    }

    private IActionResult? VerificarAccesoReportes()
    {
        var sesion = RedirigirSiNoHaySesion();
        if (sesion != null) return sesion;

        var rol = HttpContext.Session.GetString("Rol");
        if (rol == "Recepcionista")
        {
            TempData["Error"] = "Tu rol no tiene acceso a los reportes del sistema.";
            return RedirectToAction("Index", "Home");
        }
        return null;
    }

    private bool PuedeVerReporte(string[] rolesPermitidos)
    {
        var rol = HttpContext.Session.GetString("Rol") ?? "";
        return rolesPermitidos.Contains(rol);
    }

    // ── ÍNDICE: Catálogo de reportes disponibles ────────────────────────────

    public IActionResult Index()
    {
        var guard = VerificarAccesoReportes();
        if (guard != null) return guard;

        var rol = HttpContext.Session.GetString("Rol");
        ViewData["Title"] = "Reportes";
        ViewBag.Rol = rol;
        return View();
    }

    // ── REPORTE 1: Ocupación ────────────────────────────────────────────────
    // Disponible: Administrador, Supervisor

    public async Task<IActionResult> Ocupacion()
    {
        if (!PuedeVerReporte(["Administrador", "Supervisor"]))
        {
            TempData["Error"] = "Sin permiso para este reporte.";
            return RedirectToAction("Index");
        }

        var client = _http.CreateClient("HotelAPI");
        var res    = await client.GetAsync("reportes/ocupacion");
        ViewData["Title"] = "Reporte de Ocupación";

        if (res.IsSuccessStatusCode)
        {
            var body  = await res.Content.ReadAsStringAsync();
            var datos = JsonSerializer.Deserialize<dynamic>(body, _json);
            ViewBag.DatosJson = body; // para usar en JavaScript del View
        }

        return View();
    }

    // ── REPORTE 2: Ingresos ─────────────────────────────────────────────────
    // Disponible: Administrador, Supervisor

    public async Task<IActionResult> Ingresos(
        [FromQuery] string? desde,
        [FromQuery] string? hasta)
    {
        if (!PuedeVerReporte(["Administrador", "Supervisor"]))
        {
            TempData["Error"] = "Sin permiso para este reporte.";
            return RedirectToAction("Index");
        }

        var client = _http.CreateClient("HotelAPI");
        var url    = $"reportes/ingresos";
        var qs     = new List<string>();
        if (!string.IsNullOrEmpty(desde)) qs.Add($"desde={desde}");
        if (!string.IsNullOrEmpty(hasta)) qs.Add($"hasta={hasta}");
        if (qs.Any()) url += "?" + string.Join("&", qs);

        var res = await client.GetAsync(url);
        ViewData["Title"] = "Reporte de Ingresos";
        ViewBag.Desde = desde;
        ViewBag.Hasta = hasta;

        if (res.IsSuccessStatusCode)
            ViewBag.DatosJson = await res.Content.ReadAsStringAsync();

        return View();
    }

    // ── REPORTE 3: Reservaciones ────────────────────────────────────────────
    // Disponible: Administrador, Supervisor

    public async Task<IActionResult> Reservaciones(
        [FromQuery] string? desde,
        [FromQuery] string? hasta)
    {
        if (!PuedeVerReporte(["Administrador", "Supervisor"]))
        {
            TempData["Error"] = "Sin permiso para este reporte.";
            return RedirectToAction("Index");
        }

        var client = _http.CreateClient("HotelAPI");
        var url    = "reportes/reservaciones";
        var qs     = new List<string>();
        if (!string.IsNullOrEmpty(desde)) qs.Add($"desde={desde}");
        if (!string.IsNullOrEmpty(hasta)) qs.Add($"hasta={hasta}");
        if (qs.Any()) url += "?" + string.Join("&", qs);

        var res = await client.GetAsync(url);
        ViewData["Title"] = "Listado de Reservaciones";
        ViewBag.Desde = desde;
        ViewBag.Hasta = hasta;

        if (res.IsSuccessStatusCode)
            ViewBag.DatosJson = await res.Content.ReadAsStringAsync();

        return View();
    }

    // ── REPORTE 4: Clientes ─────────────────────────────────────────────────
    // Disponible: Administrador, Supervisor

    public async Task<IActionResult> Clientes()
    {
        if (!PuedeVerReporte(["Administrador", "Supervisor"]))
        {
            TempData["Error"] = "Sin permiso para este reporte.";
            return RedirectToAction("Index");
        }

        var client = _http.CreateClient("HotelAPI");
        var res    = await client.GetAsync("reportes/clientes");
        ViewData["Title"] = "Reporte de Clientes";

        if (res.IsSuccessStatusCode)
            ViewBag.DatosJson = await res.Content.ReadAsStringAsync();

        return View();
    }

    // ── REPORTE 5: Empleados ────────────────────────────────────────────────
    // Solo Administrador

    public async Task<IActionResult> Empleados()
    {
        if (!PuedeVerReporte(["Administrador"]))
        {
            TempData["Error"] = "Solo el Administrador puede ver este reporte.";
            return RedirectToAction("Index");
        }

        var client = _http.CreateClient("HotelAPI");
        var res    = await client.GetAsync("reportes/empleados");
        ViewData["Title"] = "Reporte de Empleados";

        if (res.IsSuccessStatusCode)
            ViewBag.DatosJson = await res.Content.ReadAsStringAsync();

        return View();
    }

    // ── REPORTE 6: Inventario de Habitaciones ───────────────────────────────
    // Solo Administrador

    public async Task<IActionResult> Habitaciones()
    {
        if (!PuedeVerReporte(["Administrador"]))
        {
            TempData["Error"] = "Solo el Administrador puede ver este reporte.";
            return RedirectToAction("Index");
        }

        var client = _http.CreateClient("HotelAPI");
        var res    = await client.GetAsync("reportes/habitaciones");
        ViewData["Title"] = "Inventario de Habitaciones";

        if (res.IsSuccessStatusCode)
            ViewBag.DatosJson = await res.Content.ReadAsStringAsync();

        return View();
    }

    // ── Impresión directa desde servidor ───────────────────────────────────
    // Redirige al reporte con parámetro de formato para exportación

    [HttpGet("Reportes/Exportar/{nombre}")]
    public IActionResult Exportar(string nombre, [FromQuery] string formato = "PDF")
    {
        if (!PuedeVerReporte(["Administrador", "Supervisor"]))
            return Unauthorized();

        // Aca se puede integrar Crystal Reports / SSRS / RDLC jjajaja pero no pude ._.
        // Ejemplo con RDLC (Microsoft.Reporting.NETCore):
        // var report = new LocalReport();
        // report.ReportPath = $"Reports/{nombre}.rdlc";
        // var bytes = report.Render(formato);
        // return File(bytes, "application/pdf", $"{nombre}.pdf");

        TempData["Error"] = "Exportación directa requiere configurar Crystal Reports o SSRS. Ver ReportesController.cs de la API.";
        return RedirectToAction(nombre);
    }
}
