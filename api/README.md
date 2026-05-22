# API REST - Sistema de Hotelería 🏨

## Proyecto Final - Programación I
**Universidad Mariano Gálvez de Guatemala**  
Ingeniería en Sistemas de Información y Ciencias de la Computación  
Sección F | Ing. Sebastián Hernández Gabriel

---

## Descripción

API REST desarrollada en **ASP.NET Core (.NET 10)** con **C#** para el sistema de hotelería. Este servicio permite realizar operaciones CRUD (Crear, Leer, Actualizar y Eliminar) sobre la base de datos **HotelDB** a través de endpoints HTTP.

El API actúa como intermediario entre la base de datos SQL Server y la aplicación web, proporcionando los servicios necesarios para la gestión de reservaciones de habitaciones, clientes, empleados y hoteles.

---

## Tecnologías utilizadas

| Tecnología | Descripción |
|---|---|
| ASP.NET Core (.NET 10) | Framework para el desarrollo del API REST |
| C# | Lenguaje de programación |
| Entity Framework Core | ORM para el acceso a datos |
| SQL Server / SQL Express | Base de datos relacional |
| Swagger / Swashbuckle | Documentación y prueba de endpoints |
| Visual Studio 2022 | Entorno de desarrollo (IDE) |

---

## Estructura del proyecto

```
API REST CONFIGURACION/
├── Controllers/            # Controladores del API (CRUD por entidad)
│   ├── ClienteController.cs
│   ├── EmpleadoController.cs
│   ├── HabitacionController.cs
│   ├── HotelController.cs
│   ├── ReservacionController.cs
│   ├── DetalleReservacionController.cs
│   ├── TipoHabitacionController.cs
│   └── UsuarioController.cs
├── Models/                 # Clases que representan las tablas de la BD
│   ├── Cliente.cs
│   ├── Empleado.cs
│   ├── Habitacion.cs
│   ├── Hotel.cs
│   ├── Reservacion.cs
│   ├── DetalleReservacion.cs
│   ├── TipoHabitacion.cs
│   └── Usuario.cs
├── Data/                   # Contexto de Entity Framework
│   └── HotelDbContext.cs
├── Program.cs              # Configuración principal del proyecto
├── appsettings.json        # Cadena de conexión a la base de datos
└── HotelAPI.csproj         # Archivo del proyecto
```

---

## Endpoints disponibles

### Usuario
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/Usuario` | Obtener todos los usuarios |
| POST | `/api/Usuario` | Crear un nuevo usuario |
| POST | `/api/Usuario/login` | Autenticar usuario (login) |

### Hotel
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/Hotel` | Obtener todos los hoteles |
| POST | `/api/Hotel` | Crear un nuevo hotel |
| PUT | `/api/Hotel/{id}` | Actualizar un hotel |
| DELETE | `/api/Hotel/{id}` | Eliminar un hotel |

### TipoHabitacion
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/TipoHabitacion` | Obtener todos los tipos |
| POST | `/api/TipoHabitacion` | Crear un nuevo tipo |
| PUT | `/api/TipoHabitacion/{id}` | Actualizar un tipo |
| DELETE | `/api/TipoHabitacion/{id}` | Eliminar un tipo |

### Habitacion
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/Habitacion` | Obtener todas las habitaciones |
| POST | `/api/Habitacion` | Crear una nueva habitación |
| PUT | `/api/Habitacion/{id}` | Actualizar una habitación |
| DELETE | `/api/Habitacion/{id}` | Eliminar una habitación |

### Cliente
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/Cliente` | Obtener todos los clientes |
| POST | `/api/Cliente` | Crear un nuevo cliente |
| PUT | `/api/Cliente/{id}` | Actualizar un cliente |
| DELETE | `/api/Cliente/{id}` | Eliminar un cliente |

### Empleado
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/Empleado` | Obtener todos los empleados |
| POST | `/api/Empleado` | Crear un nuevo empleado |
| PUT | `/api/Empleado/{id}` | Actualizar un empleado |
| DELETE | `/api/Empleado/{id}` | Eliminar un empleado |

### Reservacion
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/Reservacion` | Obtener todas las reservaciones |
| POST | `/api/Reservacion` | Crear una nueva reservación |
| PUT | `/api/Reservacion/{id}` | Actualizar una reservación |
| DELETE | `/api/Reservacion/{id}` | Eliminar una reservación |

### DetalleReservacion
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/DetalleReservacion` | Obtener todos los detalles |
| POST | `/api/DetalleReservacion` | Crear un nuevo detalle |
| PUT | `/api/DetalleReservacion/{id}` | Actualizar un detalle |
| DELETE | `/api/DetalleReservacion/{id}` | Eliminar un detalle |

---

## Requisitos previos

- [Visual Studio 2022](https://visualstudio.microsoft.com/) con la carga de trabajo **ASP.NET y desarrollo web**
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server o SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- Base de datos **HotelDB** creada previamente (ver repositorio de base de datos)

---

## Configuración e instalación

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/rpernillom-sys/API-REST-Hotel-PROYECTO-FINAL-PROGRAMACION-I.git
   ```

2. **Abrir el proyecto** en Visual Studio (archivo `HotelAPI.slnx`)

3. **Configurar la cadena de conexión** en `appsettings.json` según tu instancia de SQL Server:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=HotelDB;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

4. **Restaurar paquetes NuGet** (Visual Studio lo hace automáticamente al abrir)

5. **Ejecutar el proyecto** (F5 o Ctrl+F5). Swagger se abrirá automáticamente en el navegador.

---

## Paquetes NuGet

| Paquete | Versión |
|---|---|
| Microsoft.EntityFrameworkCore.SqlServer | 10.0.5 |
| Microsoft.EntityFrameworkCore.Tools | 10.0.5 |
| Swashbuckle.AspNetCore | 10.1.7 |
| Microsoft.AspNetCore.OpenApi | 10.0.4 |

---

## Base de datos

El script de creación de la base de datos se encuentra en el repositorio del equipo:  
🔗 [ProyectoFinal_Progra1 - Base de Datos](https://github.com/drabeg/ProyectoFinal_Progra1)

---

## Integrantes del equipo

| Nombre | Carné | Rol |
|---|---|---|
| Libbny Dayana Medrano Arévalo | 5190-25-24096 | Estructura y configuración del API |
| Richard Esteev Pernillo Macario | 5190-25-21234 | Lógica y controladores del API |
| Cristian Daniel Emiliano Cano Estrada | 5190-25-24608 | Desarrollo de la aplicación |
| Diego José Quevedo Vega | 5190-24-21422 | Desarrollo de la aplicación |
| Dario Alfredo Rabe Godoy | 5190-25-23683 | Base de datos |

---

## Distribución del trabajo - API REST (Segunda entrega)

| Richard Pernillo | Libbny Medrano |
|---|---|
| Crear los Controllers | Crear el proyecto en Visual Studio |
| Programar las operaciones CRUD | Configurar conexión a base de datos |
| Probar endpoints en Swagger | Crear los modelos (Models) |
