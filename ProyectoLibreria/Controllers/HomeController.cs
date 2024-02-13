using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ProyectoLibreria.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace ProyectoLibreria.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Constructor que inyecta una instancia de ILogger
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // M�todo para manejar las solicitudes de la p�gina de inicio
        public IActionResult Index()
        {
            // Obtener las reclamaciones del usuario autenticado
            ClaimsPrincipal claimsUser = HttpContext.User;
            string nombreUsuario = "";
            string fotoPerfil = "";

            // Verificar si el usuario est� autenticado
            if (claimsUser.Identity.IsAuthenticated)
            {
                // Obtener el nombre de usuario y la foto de perfil de las reclamaciones
                nombreUsuario = claimsUser.Claims.Where(c => c.Type == ClaimTypes.Name)
                    .Select(c => c.Value).SingleOrDefault();

                fotoPerfil = claimsUser.Claims.Where(c => c.Type == "FotoPerfil")
                    .Select(c => c.Value).SingleOrDefault();
            }

            // Establecer los datos de usuario en ViewData para su uso en la vista
            ViewData["nombreUsuario"] = nombreUsuario;
            ViewData["fotoPerfil"] = fotoPerfil;

            // Renderizar la vista de la p�gina de inicio
            return View();
        }

        // M�todo para manejar las solicitudes de la p�gina de privacidad
        public IActionResult Privacy()
        {
            // Renderizar la vista de la p�gina de privacidad
            return View();
        }

        // M�todo para manejar los errores
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Renderizar la vista de error con un modelo que contiene el ID de la solicitud
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // M�todo para manejar el cierre de sesi�n
        public async Task<IActionResult> CerrarSesion()
        {
            // Cerrar la sesi�n del usuario actual
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirigir a la p�gina de inicio de sesi�n
            return RedirectToAction("IniciarSesion", "Login");
        }
    }
}