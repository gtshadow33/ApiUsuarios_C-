// DTOs
namespace ApiUsuarios.Controllers;
using System.ComponentModel.DataAnnotations;
public class CreateUserDto
{
    public string Nombre { get; set; } = "";
    public string Email { get; set; } = "";
    public string Rol { get; set; } = "usuario";
    public string Password { get; set; } = "";
}



public class UpdateUserDto
{

    public string? Nombre { get; set; }


    public string? Email { get; set; }


    public string? Rol { get; set; }


    public string? Password { get; set; }
}


public class safeUserDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Email { get; set; } = "";
    public string Rol { get; set; } = "usuario";

}