# 🏨 Hotel MVC — Sistema de Gestión Hotelera

Sistema de gestión hotelera desarrollado con arquitectura de dos capas: una **API REST** construida en ASP.NET Core (.NET 10) como backend, y un **frontend MVC** separado que consume dicha API. El proyecto permite administrar reservaciones, habitaciones, clientes, empleados y más, a través de endpoints RESTful documentados con Swagger.

---

## 🛠️ Tecnologías Utilizadas

### Backend (API REST)
| Tecnología | Versión | Descripción |
|---|---|---|
| .NET / ASP.NET Core | 10.0 | Framework principal del servidor |
| Entity Framework Core | 10.0.5 | ORM para acceso a base de datos |
| EF Core SqlServer | 10.0.5 | Proveedor para SQL Server |
| Swashbuckle / Swagger | 10.1.7 | Documentación interactiva de la API |
| Microsoft.AspNetCore.OpenApi | 10.0.4 | Soporte OpenAPI |

### Base de Datos
| Tecnología | Descripción |
|---|---|
| SQL Server / SQL Server Express | Motor de base de datos relacional |
| T-SQL | Definición del esquema de tablas |

### Frontend (MVC)
- Carpeta `FRONT_HOTEL_MVC` incluida en la solución (interfaz que consume la API REST)

### Herramientas de Desarrollo
- Visual Studio 2022 (solución `.slnx`)
- Postman / Swagger UI para pruebas de endpoints

---

## 📁 Estructura del Proyecto

```
HOTEL_MVC_v1/
├── HotelMVCVISUAL.slnx                  # Solución Visual Studio principal
│
├── API_HOTEL_MAIN/                       # Proyecto de la API REST
│   ├── API REST CONFIGURACION/
│   │   ├── Controllers/                  # Controladores de la API
│   │   │   ├── ClienteController.cs
│   │   │   ├── DetalleReservacionController.cs
│   │   │   ├── EmpleadoController.cs
│   │   │   ├── HabitacionController.cs
│   │   │   ├── HotelController.cs
│   │   │   ├── ReservacionController.cs
│   │   │   ├── TipoHabitacionController.cs
│   │   │   └── UsuarioController.cs
│   │   ├── Data/
│   │   │   └── HotelDbContext.cs         # Contexto de Entity Framework
│   │   ├── Models/                       # Modelos de la base de datos
│   │   │   ├── Cliente.cs
│   │   │   ├── DetalleReservacion.cs
│   │   │   ├── Empleado.cs
│   │   │   ├── Habitacion.cs
│   │   │   ├── Hotel.cs
│   │   │   ├── Reservacion.cs
│   │   │   ├── TipoHabitacion.cs
│   │   │   └── Usuario.cs
│   │   ├── Properties/
│   │   │   └── launchSettings.json       # Configuración de perfiles de inicio
│   │   ├── appsettings.json              # Configuración de conexión y app
│   │   ├── appsettings.Development.json
│   │   ├── Program.cs                    # Punto de entrada y configuración de servicios
│   │   └── HotelAPI.csproj
│   └── HotelAPI.slnx
│
└── FRONT_HOTEL_MVC/                      # Frontend MVC (consume la API)
```

---

---

## 📁 Estructura del fRONT

```
HotelMVC/
├── Controllers/          ← Controladores MVC (C#)
│   ├── HomeController.cs      (Login, Dashboard, Logout)
│   ├── ClienteController.cs
│   ├── HotelController.cs
│   ├── HabitacionController.cs
│   ├── EmpleadoController.cs
│   ├── ReservacionController.cs
│   └── UsuarioController.cs
├── Models/
│   └── Models.cs         ← Clases que mapean los datos de la API
├── Views/                ← Vistas Razor (.cshtml)
│   ├── Shared/_Layout.cshtml  (sidebar + topbar compartido)
│   ├── Home/Login.cshtml
│   ├── Home/Index.cshtml (Dashboard)
│   └── [Cliente|Hotel|Habitacion|Empleado|Reservacion|Usuario]/
│       ├── Index.cshtml   (tabla con datos)
│       ├── Crear.cshtml   (formulario nuevo)
│       └── Editar.cshtml  (formulario editar)
├── wwwroot/css/site.css  ← Estilos dark theme
├── Program.cs
├── appsettings.json      ← URL de la API aquí
└── HotelMVC.csproj
```

---

## 🗄️ Modelo de Base de Datos

El sistema maneja las siguientes entidades relacionales:

```
Hotel ──────< Habitacion >────── TipoHabitacion
  │                │
  │                └──────< DetalleReservacion
  │                                │
  └──────< Reservacion >──────────┘
              │
        ┌─────┴─────┐
      Cliente    Empleado
                    │
                 Usuario
```

**Tablas principales:**
- `Hotel` — información del establecimiento (nombre, dirección, teléfono)
- `TipoHabitacion` — categorías de habitación con precio por noche
- `Habitacion` — habitaciones con estado (disponible/ocupada) vinculadas a hotel y tipo
- `Cliente` — datos personales (nombre, apellido, DPI, correo, teléfono)
- `Empleado` — personal del hotel (nombre, puesto, teléfono)
- `Usuario` — credenciales de acceso vinculadas a empleado, incluye campo `rol`
- `Reservacion` — registro de reservas con fechas, total y relaciones a cliente/empleado/hotel
- `DetalleReservacion` — desglose por habitación (días, subtotal)

---

## 🔌 Endpoints de la API

Todos los endpoints siguen la convención REST `api/[entidad]` y soportan operaciones CRUD completas:

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/Hotel` | Listar hoteles |
| GET | `/api/Cliente` | Listar clientes |
| GET | `/api/Habitacion` | Listar habitaciones |
| GET | `/api/TipoHabitacion` | Listar tipos de habitación |
| GET | `/api/Empleado` | Listar empleados |
| GET | `/api/Usuario` | Listar usuarios |
| GET | `/api/Reservacion` | Listar reservaciones |
| GET | `/api/DetalleReservacion` | Listar detalles de reservación |
| POST | `/api/[entidad]` | Crear nuevo registro |
| PUT | `/api/[entidad]/{id}` | Actualizar registro existente |
| DELETE | `/api/[entidad]/{id}` | Eliminar registro |

La documentación interactiva completa está disponible en `/swagger` al ejecutar el proyecto.

---

## ✅ Requisitos Previos

Antes de clonar y ejecutar el proyecto, asegúrate de tener instalado:

### Obligatorios
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server Express](https://www.microsoft.com/es-es/sql-server/sql-server-downloads) (o SQL Server completo)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recomendado) con el workload **ASP.NET and web development**

### Opcionales pero recomendados
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/es-es/sql/ssms/download-sql-server-management-studio-ssms) — para administrar la base de datos
- [Postman](https://www.postman.com/) — para probar los endpoints manualmente

---

## ⚙️ Configuración e Instalación

### 1. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/hotel-mvc.git
cd hotel-mvc
```

### 2. Configurar la base de datos

Crea la base de datos en SQL Server con el nombre `HotelDB`. Puedes ejecutar el script SQL con la estructura de las tablas (si se incluye en el repositorio) o aplicar las migraciones de Entity Framework:

```bash
cd "API_HOTEL_MAIN/API REST CONFIGURACION"
dotnet ef database update
```

### 3. Configurar la cadena de conexión

Edita el archivo `appsettings.json` en el proyecto de la API:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=HotelDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

> Ajusta `Server=` según tu instancia de SQL Server local (ej. `localhost`, `.\SQLEXPRESS`, etc.)

### 4. Ejecutar la API

Desde Visual Studio, establece `API REST CONFIGURACION` como proyecto de inicio y presiona `F5`, o desde la terminal:

```bash
cd "API_HOTEL_MAIN/API REST CONFIGURACION"
dotnet run
```

La API estará disponible en `https://localhost:{puerto}` y Swagger en `https://localhost:{puerto}/swagger`.

### 5. Ejecutar el Frontend

Abre el proyecto `FRONT_HOTEL_MVC` y configura la URL base de la API según corresponda antes de ejecutarlo.

---

## 🌐 Configuración de CORS

La API tiene CORS habilitado para aceptar peticiones desde cualquier origen durante el desarrollo:

```csharp
policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
```

Para producción se recomienda restringir los orígenes permitidos.

---

## 📄 Licencia

Este proyecto fue desarrollado con fines académicos.

Autores
Nombre: Dario Alfredo Rabe Godoy /Carné: 5190-25-23683
Nombre: Libbny Dayana Medrano Arévalo /Carné: 5190-25-24096
Nombre: Richard Esteev Pernillo Macario /Carné: 5190-25-21234
Nombre: Cristian Daniel Emiliano Cano Estrada /Carné: 5190-25-24608
Nombre: Diego Jose Quevedo Vega /Carné: 5190-24-21422
Universidad Mariano Gálvez
