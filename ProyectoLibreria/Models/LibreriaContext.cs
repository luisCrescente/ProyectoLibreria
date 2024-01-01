using Microsoft.EntityFrameworkCore;

namespace ProyectoLibreria.Models.Entity
{
    public class LibreriaContext : DbContext
    {
        public LibreriaContext(DbContextOptions<LibreriaContext> options) : base(options)
        {
        }

        public DbSet<Autor> autores { get; set; }
        public DbSet<Categoria> categorias { get; set; }
        public DbSet<Libro> libros { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<Usuario> usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Categoria>().HasIndex(c => c.Nombre).IsUnique();
        }
    }

}
