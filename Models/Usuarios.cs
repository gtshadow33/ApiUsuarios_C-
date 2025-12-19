using System.ComponentModel.DataAnnotations;

namespace ApiUsuarios.Models;

public class Usuario
{
    [Key] // Marca esta propiedad como clave primaria
    public int Id { get; set; }


    public string Nombre { get; set; } = "";


    public string Email { get; set; } = "";


    public string PasswordHash { get; set; } = "";


    public string Rol { get; set; } = "usuario";
}
