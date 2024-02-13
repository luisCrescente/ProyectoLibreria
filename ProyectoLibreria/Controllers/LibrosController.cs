using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoLibreria.Models;
using ProyectoLibreria.Models.Entity;
using ProyectoLibreria.Services;

namespace ProyectoLibreria.Controllers
{
    public class LibrosController : Controller
    {
        private readonly LibreriaContext _context;
        private readonly IServicioLista _servicioLista;
        private readonly IServicioImagen _servicioImagen;
        private readonly IServicioUsuario _servicioUsuario;

        // Constructor del controlador de Libros
        public LibrosController(LibreriaContext context, IServicioLista servicioLista, IServicioImagen servicioImagen, IServicioUsuario servicioUsuario)
        {
            // Inicializa los servicios y el contexto de la base de datos
            _context = context;
            _servicioLista = servicioLista;
            _servicioImagen = servicioImagen;
            _servicioUsuario = servicioUsuario;
        }

        // Acción para mostrar la lista de libros
        public async Task<IActionResult> Lista()
        {
            // Retorna la vista con la lista de libros cargada desde la base de datos, incluyendo las categorías y autores relacionados
            return View(await _context.Libros
                .Include(l => l.Categoria)
                .Include(l => l.Autor)
                .ToListAsync());
        }

        // Acción para mostrar el formulario de creación de un nuevo libro
        public async Task<IActionResult> Crear()
        {
            // Crea un nuevo objeto Libro y carga las listas de categorías y autores disponibles para mostrar en el formulario de creación
            Libro libro = new()
            {
                Categorias = await _servicioLista.GetListaCategorias(),
                Autores = await _servicioLista.GetListaAutores(),
            };

            // Retorna la vista de creación con el nuevo objeto Libro
            return View(libro);
        }

        [HttpPost]
        // Método para crear un nuevo libro
        public async Task<IActionResult> Crear(Libro libro, IFormFile Imagen)
        {
            // Verifica si el modelo es válido
            if (ModelState.IsValid)
            {
                // Abre el flujo de la imagen
                Stream image = Imagen.OpenReadStream();
                // Sube la imagen y obtiene su URL
                string urlimagen = await _servicioImagen.SubirImagen(image, Imagen.FileName);

                try
                {
                    // Asigna la URL de la imagen al libro
                    libro.URLImagen = urlimagen;

                    // Agrega el libro a la base de datos
                    _context.Add(libro);
                    await _context.SaveChangesAsync();

                    // Configura un mensaje de éxito
                    TempData["AlertMessage"] = "Libro creado exitosamente!!!";

                    // Redirecciona a la lista de libros
                    return RedirectToAction("Lista");
                }
                catch
                {
                    // Si ocurre un error al guardar en la base de datos, agrega un mensaje de error al modelo
                    ModelState.AddModelError(String.Empty, "Ha ocurrido un error");
                }
            }

            // Si el modelo no es válido, carga las listas de categorías y autores para mostrar en la vista de creación
            libro.Categorias = await _servicioLista.GetListaCategorias();
            libro.Autores = await _servicioLista.GetListaAutores();

            // Muestra la vista de creación con los datos del libro
            return View(libro);
        }

        // Método para mostrar la vista de edición de un libro
        public async Task<IActionResult> Editar(int? id)
        {
            // Verifica si el ID del libro es nulo
            if (id == null)
            {
                // Si es nulo, devuelve NotFound
                return NotFound();
            }

            // Busca el libro por su ID en la base de datos
            var libro = await _context.Libros.FindAsync(id);

            // Si el libro no existe, devuelve NotFound
            if (libro == null)
            {
                return NotFound();
            }

            // Carga las listas de categorías y autores para mostrar en la vista de edición
            libro.Categorias = await _servicioLista.GetListaCategorias();
            libro.Autores = await _servicioLista.GetListaAutores();

            // Muestra la vista de edición con los datos del libro
            return View(libro);
        }

        [HttpPost]
        // Método para editar un libro
        public async Task<IActionResult> Editar(Libro libro, IFormFile Imagen)
        {
            // Verifica si el modelo es válido
            if (ModelState.IsValid)
            {
                try
                {
                    // Busca el libro existente por su ID
                    var libroExistente = await _context.Libros.FindAsync(libro.Id);

                    // Si el libro no existe, devuelve NotFound
                    if (libroExistente == null)
                    {
                        return NotFound();
                    }

                    // Si se proporciona una nueva imagen, la sube y actualiza la URL de la imagen
                    if (Imagen != null)
                    {
                        Stream image = Imagen.OpenReadStream();
                        string urlimagen = await _servicioImagen.SubirImagen(image, Imagen.FileName);
                        libroExistente.URLImagen = urlimagen;
                    }

                    // Actualiza los otros datos del libro
                    libroExistente.Titulo = libro.Titulo;
                    libroExistente.Autor = await _context.Autores.FindAsync(libro.AutorId);
                    libroExistente.Categoria = await _context.Categorias.FindAsync(libro.CategoriaId);
                    libroExistente.Precio = libro.Precio;

                    // Actualiza el libro en la base de datos y guarda los cambios
                    _context.Update(libroExistente);
                    await _context.SaveChangesAsync();
                    TempData["AlertMessage"] = "Libro actualizado exitosamente";

                    // Redirecciona a la acción Lista después de editar el libro
                    return RedirectToAction("Lista");
                }
                catch
                {
                    // Si ocurre un error, agrega un error al modelo de errores
                    ModelState.AddModelError(string.Empty, "Ha ocurrido un error");
                }
            }

            // Si el modelo no es válido, vuelve a cargar la vista de edición con el libro y las listas de categorías y autores
            libro.Categorias = await _servicioLista.GetListaCategorias();
            libro.Autores = await _servicioLista.GetListaAutores();

            return View(libro);
        }

        // Método para eliminar un libro
        public async Task<IActionResult> Eliminar(int? id)
        {
            // Verifica si el ID es nulo o si no hay libros en el contexto
            if (id == null || _context.Libros == null)
            {
                return NotFound();
            }

            // Busca el libro por su ID
            var libro = await _context.Libros.FirstOrDefaultAsync(m => m.Id == id);

            // Si no se encuentra el libro, devuelve NotFound
            if (libro == null)
            {
                return NotFound();
            }

            try
            {
                // Intenta eliminar el libro y guardar los cambios en la base de datos
                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = "Libro eliminado exitosamente";
            }
            catch (Exception ex)
            {
                // Si ocurre un error al eliminar, agrega un error al modelo de errores
                ModelState.AddModelError(ex.Message, "Ocurrio un error, no se pudo eliminar el registro");
            }

            // Redirecciona a la acción Lista después de eliminar el libro
            return RedirectToAction(nameof(Lista));
        }
    }
}
