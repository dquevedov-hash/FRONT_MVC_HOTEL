var builder = WebApplication.CreateBuilder(args);

// Registra MVC con vistas Razor
builder.Services.AddControllersWithViews();

// HttpClient para consumir la API REST
builder.Services.AddHttpClient("HotelAPI", client =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000/api";
    client.BaseAddress = new Uri(baseUrl + "/");
});

// Sesión para guardar el usuario logueado
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
