using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoLibreria.Models.Entity;

namespace ProyectoLibreria.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly LibreriaContext _context;

        public CategoriasController(LibreriaContext context)
        {
            _context = context;
        }

        // Método para mostrar la lista de categorías
        public async Task<IActionResult> Lista()
        {
            return View(await _context.Categorias.ToListAsync());
        }

        // Método para mostrar el formulario de creación de categoría
        public IActionResult Crear()
        {
            return View();
        }

        // Método para procesar el formulario de creación de categoría
        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(categoria);
                    await _context.SaveChangesAsync();
                    TempData["AlertMessage"] = "¡Categoría creada exitosamente!";
                    return RedirectToAction("Lista");
                }
                catch
                {
                    ModelState.AddModelError(String.Empty, "Ha ocurrido un error al crear la categoría");
                }
            }
            return View(categoria);
        }

        // Método para mostrar el formulario de edición de categoría
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null || _context.Categorias == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // Método para procesar el formulario de edición de categoría
        [HttpPost]
        public async Task<IActionResult> Editar(int id, Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                    TempData["AlertMessage"] = "¡Categoría actualizada exitosamente!";
                    return RedirectToAction("Lista");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(ex.Message, "Ha ocurrido un error al actualizar la categoría");
                }
            }
            return View(categoria);
        }

        // Método para eliminar una categoría
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias.FirstOrDefaultAsync(m => m.Id == id);

            if (categoria == null)
            {
                return NotFound();
            }

            try
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = "¡Categoría eliminada exitosamente!";
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(ex.Message, "Ha ocurrido un error al eliminar la categoría");
            }

            return RedirectToAction(nameof(Lista));
        }
    }
}