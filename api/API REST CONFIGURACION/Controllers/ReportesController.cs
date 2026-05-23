/*
 * ReportesController.cs
 * ─────────────────────────────────────────────────────────────────────────────
 * Controlador de Reportes para la API REST del Hotel OS.
 *
 * INSTRUCCIONES DE INTEGRACIÓN:
 * 1. Colocar este archivo en:
 *      API REST CONFIGURACION/Controllers/ReportesController.cs
 *
 * 2. El frontend (app.js) ya consume los endpoints /api/reservacion,
 *    /api/habitacion, etc. directamente. Este controlador agrega endpoints
 *    especializados de reportes con filtros por fecha y agrupaciones.
 *
 * 3. Para usar Crystal Reports o SSRS en producción, ver la sección
 *    "Integración con Crystal Reports / SSRS" al final del archivo.
 * ─────────────────────────────────────────────────────────────────────────────
 */

using Microsoft.AspNetCore.Mvc;
using HotelAPI.Data;
using HotelAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public ReportesController(HotelDbContext context)
        {
            _context = context;
        }

        // ── REPORTE 1: OCUPACIÓN ─────────────────────────────────────────────
        // GET: api/reportes/ocupacion
        // Devuelve estado de habitaciones agrupadas por hotel.
        [HttpGet("ocupacion")]
        public async Task<IActionResult> GetOcupacion()
        {
            var hoteles = await _context.Hotels.ToListAsync();
            var habitaciones = await _context.Habitacions
                .Include(h => h.IdTipoNavigation)
                .ToListAsync();

            var resultado = hoteles.Select(hotel => new
            {
                hotel.IdHotel,
                hotel.Nombre,
                hotel.Direccion,
                TotalHabitaciones = habitaciones.Count(h => h.IdHotel == hotel.IdHotel),
                Disponibles   = habitaciones.Count(h => h.IdHotel == hotel.IdHotel && h.Estado == "Disponible"),
                Ocupadas      = habitaciones.Count(h => h.IdHotel == hotel.IdHotel && h.Estado == "Ocupada"),
                Mantenimiento = habitaciones.Count(h => h.IdHotel == hotel.IdHotel && h.Estado == "Mantenimiento"),
                PorcentajeOcupacion = habitaciones.Count(h => h.IdHotel == hotel.IdHotel) > 0
                    ? Math.Round((double)habitaciones.Count(h => h.IdHotel == hotel.IdHotel && h.Estado == "Ocupada")
                        / habitaciones.Count(h => h.IdHotel == hotel.IdHotel) * 100, 1)
                    : 0,
                Habitaciones = habitaciones
                    .Where(h => h.IdHotel == hotel.IdHotel)
                    .Select(h => new
                    {
                        h.IdHabitacion,
                        h.Numero,
                        h.Estado,
                        Tipo = h.IdTipoNavigation?.Nombre ?? "Sin tipo"
                    }).ToList()
            });

            // Totales globales
            var resumen = new
            {
                FechaGeneracion    = DateTime.Now,
                TotalHoteles       = hoteles.Count,
                TotalHabitaciones  = habitaciones.Count,
                TotalDisponibles   = habitaciones.Count(h => h.Estado == "Disponible"),
                TotalOcupadas      = habitaciones.Count(h => h.Estado == "Ocupada"),
                TotalMantenimiento = habitaciones.Count(h => h.Estado == "Mantenimiento"),
                PorcentajeOcupacionGlobal = habitaciones.Count > 0
                    ? Math.Round((double)habitaciones.Count(h => h.Estado == "Ocupada")
                        / habitaciones.Count * 100, 1)
                    : 0,
                PorHotel = resultado
            };

            return Ok(resumen);
        }

        // ── REPORTE 2: INGRESOS ──────────────────────────────────────────────
        // GET: api/reportes/ingresos?desde=2025-01-01&hasta=2025-12-31
        // Devuelve ingresos totales y por hotel, con filtro opcional de fechas.
        [HttpGet("ingresos")]
        public async Task<IActionResult> GetIngresos(
            [FromQuery] DateOnly? desde,
            [FromQuery] DateOnly? hasta)
        {
            var query = _context.Reservacions
                .Include(r => r.IdHotelNavigation)
                .Include(r => r.IdClienteNavigation)
                .AsQueryable();

            if (desde.HasValue) query = query.Where(r => r.FechaInicio >= desde.Value);
            if (hasta.HasValue) query = query.Where(r => r.FechaInicio <= hasta.Value);

            var reservaciones = await query.ToListAsync();
            var hoteles       = await _context.Hotels.ToListAsync();

            var ingresosPorHotel = hoteles.Select(h => new
            {
                h.IdHotel,
                h.Nombre,
                TotalReservaciones = reservaciones.Count(r => r.IdHotel == h.IdHotel),
                IngresoTotal       = reservaciones
                    .Where(r => r.IdHotel == h.IdHotel)
                    .Sum(r => r.Total ?? 0),
            }).OrderByDescending(x => x.IngresoTotal).ToList();

            var totalIngresos = reservaciones.Sum(r => r.Total ?? 0);

            var resultado = new
            {
                FechaGeneracion   = DateTime.Now,
                Desde             = desde,
                Hasta             = hasta,
                TotalReservaciones= reservaciones.Count,
                IngresoTotal      = totalIngresos,
                PromedioPorReserva= reservaciones.Count > 0
                    ? Math.Round(totalIngresos / reservaciones.Count, 2)
                    : 0,
                PorHotel = ingresosPorHotel.Select(h => new
                {
                    h.IdHotel, h.Nombre, h.TotalReservaciones, h.IngresoTotal,
                    PorcentajeDelTotal = totalIngresos > 0
                        ? Math.Round((double)h.IngresoTotal / (double)totalIngresos * 100, 1)
                        : 0
                }),
                Detalle = reservaciones
                    .OrderByDescending(r => r.FechaInicio)
                    .Select(r => new
                    {
                        r.IdReservacion,
                        Cliente  = r.IdClienteNavigation != null
                            ? $"{r.IdClienteNavigation.Nombre} {r.IdClienteNavigation.Apellido}"
                            : $"#{r.IdCliente}",
                        Hotel    = r.IdHotelNavigation?.Nombre ?? $"#{r.IdHotel}",
                        r.FechaInicio,
                        r.FechaFin,
                        Noches   = (r.FechaFin.DayNumber - r.FechaInicio.DayNumber),
                        r.Total,
                    })
            };

            return Ok(resultado);
        }

        // ── REPORTE 3: RESERVACIONES ─────────────────────────────────────────
        // GET: api/reportes/reservaciones?desde=2025-01-01&hasta=2025-12-31
        [HttpGet("reservaciones")]
        public async Task<IActionResult> GetReservaciones(
            [FromQuery] DateOnly? desde,
            [FromQuery] DateOnly? hasta)
        {
            var query = _context.Reservacions
                .Include(r => r.IdClienteNavigation)
                .Include(r => r.IdHotelNavigation)
                .Include(r => r.IdEmpleadoNavigation)
                .AsQueryable();

            if (desde.HasValue) query = query.Where(r => r.FechaInicio >= desde.Value);
            if (hasta.HasValue) query = query.Where(r => r.FechaInicio <= hasta.Value);

            var reservaciones = await query
                .OrderByDescending(r => r.FechaInicio)
                .ToListAsync();

            var resultado = new
            {
                FechaGeneracion    = DateTime.Now,
                Desde              = desde,
                Hasta              = hasta,
                TotalReservaciones = reservaciones.Count,
                IngresoTotal       = reservaciones.Sum(r => r.Total ?? 0),
                Detalle = reservaciones.Select(r => new
                {
                    r.IdReservacion,
                    Cliente  = r.IdClienteNavigation != null
                        ? $"{r.IdClienteNavigation.Nombre} {r.IdClienteNavigation.Apellido}"
                        : $"#{r.IdCliente}",
                    Empleado = r.IdEmpleadoNavigation != null
                        ? $"{r.IdEmpleadoNavigation.Nombre} {r.IdEmpleadoNavigation.Apellido}"
                        : $"#{r.IdEmpleado}",
                    Hotel    = r.IdHotelNavigation?.Nombre ?? $"#{r.IdHotel}",
                    r.FechaInicio,
                    r.FechaFin,
                    Noches   = (r.FechaFin.DayNumber - r.FechaInicio.DayNumber),
                    r.Total,
                })
            };

            return Ok(resultado);
        }

        // ── REPORTE 4: CLIENTES ──────────────────────────────────────────────
        // GET: api/reportes/clientes
        [HttpGet("clientes")]
        public async Task<IActionResult> GetClientes()
        {
            var clientes      = await _context.Clientes.ToListAsync();
            var reservaciones = await _context.Reservacions.ToListAsync();

            var detalle = clientes.Select(c => new
            {
                c.IdCliente,
                c.Nombre,
                c.Apellido,
                c.Dpi,
                c.Telefono,
                c.Correo,
                TotalReservaciones = reservaciones.Count(r => r.IdCliente == c.IdCliente),
                GastoTotal         = reservaciones
                    .Where(r => r.IdCliente == c.IdCliente)
                    .Sum(r => r.Total ?? 0)
            }).OrderByDescending(c => c.GastoTotal).ToList();

            return Ok(new
            {
                FechaGeneracion       = DateTime.Now,
                TotalClientes         = clientes.Count,
                ClientesConReservas   = detalle.Count(c => c.TotalReservaciones > 0),
                IngresosTotalesClientes = detalle.Sum(c => c.GastoTotal),
                Detalle = detalle
            });
        }

        // ── REPORTE 5: EMPLEADOS ─────────────────────────────────────────────
        // GET: api/reportes/empleados
        [HttpGet("empleados")]
        public async Task<IActionResult> GetEmpleados()
        {
            var empleados = await _context.Empleados.ToListAsync();

            var porPuesto = empleados
                .GroupBy(e => e.Puesto ?? "Sin puesto")
                .Select(g => new
                {
                    Puesto    = g.Key,
                    Cantidad  = g.Count(),
                    Empleados = g.Select(e => new
                    {
                        e.IdEmpleado, e.Nombre, e.Apellido, e.Puesto, e.Telefono
                    }).ToList()
                }).OrderByDescending(g => g.Cantidad).ToList();

            return Ok(new
            {
                FechaGeneracion  = DateTime.Now,
                TotalEmpleados   = empleados.Count,
                TotalPuestos     = porPuesto.Count,
                PorPuesto        = porPuesto
            });
        }

        // ── REPORTE 6: INVENTARIO DE HABITACIONES ────────────────────────────
        // GET: api/reportes/habitaciones
        [HttpGet("habitaciones")]
        public async Task<IActionResult> GetHabitaciones()
        {
            var hoteles       = await _context.Hotels.ToListAsync();
            var habitaciones  = await _context.Habitacions
                .Include(h => h.IdTipoNavigation)
                .ToListAsync();
            var tipos         = await _context.TipoHabitacions.ToListAsync();

            var porHotel = hoteles.Select(hotel => new
            {
                hotel.IdHotel,
                hotel.Nombre,
                hotel.Direccion,
                Total         = habitaciones.Count(h => h.IdHotel == hotel.IdHotel),
                Disponibles   = habitaciones.Count(h => h.IdHotel == hotel.IdHotel && h.Estado == "Disponible"),
                Ocupadas      = habitaciones.Count(h => h.IdHotel == hotel.IdHotel && h.Estado == "Ocupada"),
                Mantenimiento = habitaciones.Count(h => h.IdHotel == hotel.IdHotel && h.Estado == "Mantenimiento"),
                Habitaciones  = habitaciones
                    .Where(h => h.IdHotel == hotel.IdHotel)
                    .Select(h => new
                    {
                        h.IdHabitacion,
                        h.Numero,
                        h.Estado,
                        Tipo  = h.IdTipoNavigation?.Nombre ?? "—",
                        Precio= h.IdTipoNavigation?.Precio ?? 0
                    }).ToList()
            }).ToList();

            return Ok(new
            {
                FechaGeneracion   = DateTime.Now,
                TotalHabitaciones = habitaciones.Count,
                TotalHoteles      = hoteles.Count,
                TotalTipos        = tipos.Count,
                PorHotel          = porHotel,
                Tipos             = tipos.Select(t => new
                {
                    t.IdTipo, t.Nombre, t.Descripcion, t.Precio,
                    Cantidad = habitaciones.Count(h => h.IdTipo == t.IdTipo)
                })
            });
        }

        // ── REPORTE CONSOLIDADO ──────────────────────────────────────────────
        // GET: api/reportes/dashboard
        // Resumen ejecutivo para el Dashboard del Supervisor y Admin.
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var habitaciones  = await _context.Habitacions.ToListAsync();
            var reservaciones = await _context.Reservacions.ToListAsync();
            var clientes      = await _context.Clientes.ToListAsync();
            var empleados     = await _context.Empleados.ToListAsync();
            var hoteles       = await _context.Hotels.ToListAsync();

            var total     = habitaciones.Count;
            var ocupadas  = habitaciones.Count(h => h.Estado == "Ocupada");
            var disponib  = habitaciones.Count(h => h.Estado == "Disponible");
            var mantenim  = habitaciones.Count(h => h.Estado == "Mantenimiento");

            return Ok(new
            {
                FechaGeneracion    = DateTime.Now,
                TotalHoteles       = hoteles.Count,
                TotalHabitaciones  = total,
                HabDisponibles     = disponib,
                HabOcupadas        = ocupadas,
                HabMantenimiento   = mantenim,
                PorcentajeOcupacion= total > 0 ? Math.Round((double)ocupadas / total * 100, 1) : 0,
                TotalClientes      = clientes.Count,
                TotalEmpleados     = empleados.Count,
                TotalReservaciones = reservaciones.Count,
                IngresosTotales    = reservaciones.Sum(r => r.Total ?? 0),
                IngresosPromedio   = reservaciones.Count > 0
                    ? Math.Round(reservaciones.Sum(r => r.Total ?? 0) / reservaciones.Count, 2)
                    : 0,
                UltimasReservaciones = reservaciones
                    .OrderByDescending(r => r.FechaInicio)
                    .Take(5)
                    .Select(r => new { r.IdReservacion, r.IdCliente, r.IdHotel, r.FechaInicio, r.FechaFin, r.Total })
            });
        }
    }
}

/*
 * ═══════════════════════════════════════════════════════════════════════════
 *  INTEGRACIÓN CON CRYSTAL REPORTS / SQL SERVER REPORTING SERVICES (SSRS)
 * ═══════════════════════════════════════════════════════════════════════════
 *
 *  La arquitectura del sistema soporta dos enfoques para reportes avanzados:
 *
 *  ── OPCIÓN A: Crystal Reports (para proyectos .NET Framework / .NET 6+) ──
 *
 *  1. Instalar el paquete NuGet:
 *       CrystalDecisions.CrystalReports.Engine
 *       CrystalDecisions.Web
 *
 *  2. Crear el archivo .rpt en Visual Studio con Crystal Reports Designer.
 *
 *  3. Agregar este endpoint al controlador:
 *
 *       [HttpGet("crystal/ocupacion")]
 *       public IActionResult DescargarCrystalOcupacion()
 *       {
 *           var reporte = new ReporteOcupacion(); // clase generada por CR
 *           var datos   = _context.Habitaciones
 *               .Include(h => h.IdTipoNavigation)
 *               .ToList();
 *
 *           reporte.SetDataSource(datos);
 *
 *           var stream = reporte.ExportToStream(
 *               CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
 *
 *           return File(stream, "application/pdf", "reporte_ocupacion.pdf");
 *       }
 *
 *  4. En el frontend (app.js), en la función exportPDF(), reemplazar:
 *       window.open(`${API}/reportes/crystal/ocupacion`)
 *
 * ── OPCIÓN B: SQL Server Reporting Services (SSRS) ──────────────────────
 *
 *  1. Diseñar el reporte (.rdl) en SQL Server Data Tools (SSDT).
 *
 *  2. Publicar el .rdl en el servidor de SSRS (Report Server).
 *
 *  3. Agregar el endpoint en este controlador:
 *
 *       [HttpGet("ssrs/ocupacion")]
 *       public async Task<IActionResult> DescargarSSRSOcupacion(
 *           [FromQuery] string formato = "PDF")
 *       {
 *           // URL del servidor SSRS + parámetros
 *           var ssrsUrl = $"http://TU_SERVIDOR/ReportServer?" +
 *               $"%2fHotelOS%2fReporteOcupacion&rs:Command=Render" +
 *               $"&rs:Format={formato}";
 *
 *           using var http = new HttpClient();
 *           // Si SSRS requiere autenticación Windows:
 *           // http = new HttpClient(new HttpClientHandler {
 *           //     UseDefaultCredentials = true });
 *
 *           var bytes = await http.GetByteArrayAsync(ssrsUrl);
 *           var mime  = formato == "EXCEL"
 *               ? "application/vnd.ms-excel"
 *               : "application/pdf";
 *
 *           return File(bytes, mime, $"reporte.{formato.ToLower()}");
 *       }
 *
 *  4. En el frontend, agregar el botón correspondiente:
 *       <button onclick="window.open(`${API}/reportes/ssrs/ocupacion?formato=PDF`)">
 *         📄 Descargar PDF (SSRS)
 *       </button>
 *
 * ── OPCIÓN C: FastReport / Telerik Reporting (alternativas modernas) ─────
 *
 *  Estas librerías funcionan con .NET 8/10 sin restricciones:
 *    - FastReport.Core (NuGet: FastReport.OpenSource.Core)
 *    - Telerik.Reporting (requiere licencia)
 *    - RDLC con Microsoft.Reporting.NETCore
 *
 *  Instalar RDLC (gratuito, compatible con .rdlc de SSRS):
 *    dotnet add package Microsoft.Reporting.NETCore
 *
 *  Uso básico:
 *    var report = new LocalReport();
 *    report.ReportPath = "Reports/Ocupacion.rdlc";
 *    report.DataSources.Add(new ReportDataSource("Habitaciones", datos));
 *    byte[] pdf = report.Render("PDF");
 *    return File(pdf, "application/pdf", "ocupacion.pdf");
 *
 * ═══════════════════════════════════════════════════════════════════════════
 */
