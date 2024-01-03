﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoLibreria.Models.Entity;

namespace ProyectoLibreria.Controllers
{
    public class AutoresController : Controller
    {
        private readonly LibreriaContext _context;

        public AutoresController(LibreriaContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Lista()
        {
            return View(await _context.Autores.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Autor autor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(autor);
                    await _context.SaveChangesAsync();
                    TempData["AlertMessage"] = "Autor creado exitosamente!!!";
                    return RedirectToAction("Lista");
                }
                catch
                {
                    ModelState.AddModelError(String.Empty, "Ha ocurrido un error");
                }
            }
            return View(autor);
        }


        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null || _context.Autores == null)
            {
                return NotFound();
            }

            var autor = await _context.Autores.FindAsync(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        [HttpPost]

        public async Task<IActionResult> Editar(int id, Autor autor)
        {
            if(id != autor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(autor);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Lista");
                }
                catch ( Exception ex)
                {
                    ModelState.AddModelError(ex.Message, "Ocurrio un error al actualizar ");
                }
            }
                return View(autor);
        }
    }
}
