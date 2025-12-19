using System.ComponentModel.DataAnnotations;

namespace ApiUsuarios.Models;

public class LoginRequest
{

    public string Email { get; set; } = null!;

 
    public string Password { get; set; } = null!;
}
