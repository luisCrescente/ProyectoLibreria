using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoLibreria.Models.Entity;
using System;
using System.Threading.Tasks;

namespace ProyectoLibreria.Controllers
{
    public class AutoresController : Controller
    {
        private readonly LibreriaContext _context;

        public AutoresController(LibreriaContext context)
        {
            _context = context;
        }

        // Método para mostrar la lista de autores
        public async Task<IActionResult> Lista()
        {
            return View(await _context.Autores.ToListAsync());
        }

        // Método para mostrar el formulario de creación de autor
        public IActionResult Crear()
        {
            return View();
        }

        // Método para procesar la creación de un nuevo autor
        [HttpPost]
        public async Task<IActionResult> Crear(Autor autor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Agrega el nuevo autor a la base de datos y redirige a la lista de autores
                    _context.Add(autor);
                    await _context.SaveChangesAsync();
                    TempData["AlertMessage"] = "Autor creado exitosamente!!!";
                    return RedirectToAction("Lista");
                }
                catch
                {
                    // Maneja cualquier error durante la creación del autor
                    ModelState.AddModelError(String.Empty, "Ha ocurrido un error");
                }
            }
            return View(autor);
        }

        // Método para mostrar el formulario de edición de autor
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null || _context.Autores == null)
            {
                return NotFound();
            }

            // Busca el autor por su ID y lo muestra en el formulario de edición
            var autor = await _context.Autores.FindAsync(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        // Método para procesar la edición de un autor existente
        [HttpPost]
        public async Task<IActionResult> Editar(int id, Autor autor)
        {
            if (id != autor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualiza el autor en la base de datos y redirige a la lista de autores
                    _context.Update(autor);
                    await _context.SaveChangesAsync();
                    TempData["AlertMessage"] = "Autor actualizado exitosamente";
                    return RedirectToAction("Lista");
                }
                catch (Exception ex)
                {
                    // Maneja cualquier error durante la edición del autor
                    ModelState.AddModelError(ex.Message, "Ocurrió un error al actualizar ");
                }
            }
            return View(autor);
        }

        // Método para eliminar un autor
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null || _context.Autores == null)
            {
                return NotFound();
            }

            // Busca el autor por su ID y lo elimina de la base de datos
            var autor = await _context.Autores.FirstOrDefaultAsync(m => m.Id == id);
            if (autor == null)
            {
                return NotFound();
            }

            try
            {
                _context.Autores.Remove(autor);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = "Autor eliminado exitosamente";
            }
            catch (Exception ex)
            {
                // Maneja cualquier error durante la eliminación del autor
                ModelState.AddModelError(ex.Message, "Ocurrió un error, no se pudo eliminar el registro");
            }
            return RedirectToAction(nameof(Lista));
        }
    }
}