using Microsoft.EntityFrameworkCore;
using ProyectoLibreria.Models;
using ProyectoLibreria.Models.Entity;

namespace ProyectoLibreria.Services
{
    public class ServicioUsuario : IServicioUsuario
    {
        private readonly LibreriaContext _context;

        public ServicioUsuario(LibreriaContext context)
        {
            _context = context;
        }

         async Task<Usuario> IServicioUsuario.GetUsuario(string correo, string clave)
        {
            Usuario usuario = await _context.Usuarios.Where(u => u.Correo == correo && u.Clave == clave)
                .FirstOrDefaultAsync();

                return usuario;
        }

          async Task<Usuario> IServicioUsuario.GetUsuario(string nombreUsuario)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);
        }

         async Task<Usuario> IServicioUsuario.SaveUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }
    }
}
