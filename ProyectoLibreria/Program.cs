using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using ProyectoLibreria.Models.Entity;
using ProyectoLibreria.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<LibreriaContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL"));
});

//Se inyectan los servicios para el uso en todos los controladores de la app
builder.Services.AddScoped<IServicioLista, ServicioLista>();
builder.Services.AddScoped<IServicioImagen, ServicioImagen>();
builder.Services.AddScoped<IServicioUsuario, ServicioUsuario>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Configuraci�n del inicio de sesi�n: ruta a la p�gina de inicio de sesi�n
        options.LoginPath = "/Login/IniciarSesion";

        // Configuraci�n del tiempo de expiraci�n de la cookie de autenticaci�n
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

builder.Services.AddControllersWithViews(options =>
{
    // Configuraci�n de filtros para las vistas y controladores
    options.Filters.Add(
        new ResponseCacheAttribute
        {
            // Configuraci�n del cach� de respuesta: no almacenar en cach�
            NoStore = true,
            Location = ResponseCacheLocation.None,
        }
    );
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=IniciarSesion}/{id?}");

app.Run();
