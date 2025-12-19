using Microsoft.EntityFrameworkCore;
using ApiUsuarios.Models;

namespace ApiUsuarios.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) {}

    public DbSet<Usuario> Usuarios => Set<Usuario>();

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Usuario>(entity =>
    {
        entity.ToTable("usuarios");   // tabla en minúscula
        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.Nombre).HasColumnName("nombre");
        entity.Property(e => e.Email).HasColumnName("email");
        entity.Property(e => e.PasswordHash).HasColumnName("passwordhash");
        entity.Property(e => e.Rol).HasColumnName("rol");
    });
}
}