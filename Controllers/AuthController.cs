using Microsoft.AspNetCore.Mvc;
using ApiUsuarios.Data;
using ApiUsuarios.Models;
using ApiUsuarios.Services;
using BCrypt.Net;

namespace ApiUsuarios.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwt;

    public AuthController(AppDbContext context, JwtService jwt)
    {
        _context = context;
        _jwt = jwt;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _context.Usuarios.FirstOrDefault(u => u.Email == request.Email);
        if (user == null) 
            return Unauthorized(new { message = "Usuario no existe" });

        bool validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!validPassword)
            return Unauthorized(new { message = "Contraseña incorrecta" });

        var token = _jwt.GenerarToken(user);
        return Ok(new { token });
    }
}


public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}
