using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ProyectoLibreria.Services;
using System.Security.Claims;
using ProyectoLibreria.Models.Entity;

namespace ProyectoLibreria.Controllers
{
    public class LoginController : Controller
    {
        private readonly IServicioUsuario _servicioUsuario;
        private readonly IServicioImagen _servicioImagen;
        private readonly LibreriaContext _context;

        // Constructor que inyecta los servicios necesarios y el contexto de la base de datos
        public LoginController(IServicioUsuario servicioUsuario, IServicioImagen servicioImagen, LibreriaContext context)
        {
            _servicioUsuario = servicioUsuario;
            _servicioImagen = servicioImagen;
            _context = context;
        }

        // Método para mostrar la vista de registro de usuario
        public IActionResult Registro()
        {
            return View();
        }

        // Método para procesar el formulario de registro de usuario
        [HttpPost]
        public async Task<IActionResult> Registro(Usuario usuario, IFormFile Imagen)
        {
            // Leer la imagen del formulario y subirla al servicio de imágenes
            Stream image = Imagen.OpenReadStream();
            string urlImagen = await _servicioImagen.SubirImagen(image, Imagen.FileName);

            // Encriptar la clave del usuario y asignar la URL de la imagen de perfil
            usuario.Clave = Utilidades.EncriptarClave(usuario.Clave);
            usuario.URLFotoPerfil = urlImagen;

            // Guardar el usuario en la base de datos
            Usuario usuarioCreado = await _servicioUsuario.SaveUsuario(usuario);

            // Redireccionar a la página de inicio de sesión si el usuario se creó con éxito
            if (usuarioCreado.Id > 0)
            {
                return RedirectToAction("IniciarSesion", "Login");
            }

            // Mostrar un mensaje de error si no se pudo crear el usuario
            ViewData["Mensaje"] = "No se pudo crear el usuario";
            return View();
        }

        // Método para mostrar la vista de inicio de sesión
        public IActionResult IniciarSesion()
        {
            return View();
        }

        // Método para procesar el formulario de inicio de sesión
        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string correo, string clave)
        {
            // Buscar el usuario en la base de datos utilizando el servicio de usuario
            Usuario usuarioEncontrado = await _servicioUsuario.GetUsuario(correo, Utilidades.EncriptarClave(clave));

            // Si el usuario no se encuentra, mostrar un mensaje de error y volver a la vista de inicio de sesión
            if (usuarioEncontrado == null)
            {
                ViewData["Mensaje"] = "Usuario no encontrado";
                return View();
            }

            // Crear una lista de reclamaciones para el usuario autenticado
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuarioEncontrado.NombreUsuario),
                new Claim("FotoPerfil", usuarioEncontrado.URLFotoPerfil),
            };

            // Crear una identidad de reclamaciones y establecer el esquema de autenticación de cookies
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Configurar las propiedades de autenticación
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            // Iniciar sesión al usuario utilizando el esquema de autenticación de cookies
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            // Redirigir al usuario a la página de inicio después de iniciar sesión correctamente
            return RedirectToAction("Index", "Home");
        }
    }
}