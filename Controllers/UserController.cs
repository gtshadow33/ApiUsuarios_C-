using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiUsuarios.Data;
using ApiUsuarios.Models;
using BCrypt.Net;

namespace ApiUsuarios.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }


    // GET /users/me   → Usuario autenticado ve su propio perfil
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        int userId = int.Parse(User.FindFirst("nameidentifier")!.Value);
        var user = _context.Usuarios.Find(userId);

        if (user == null)
            return NotFound();

        safeUserDto safeUser = new safeUserDto
        {
            Id = user.Id,
            Nombre = user.Nombre,
            Email = user.Email,
            Rol = user.Rol
        };
        return Ok(safeUser);
    }

    // GET /users   → Solo admin puede ver a todos los usuarios

[Authorize(Roles = "admin")]
[HttpGet]
public IActionResult GetAll()
{
    var safeUsers = _context.Usuarios
        .Select(u => new safeUserDto
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Email = u.Email,
            Rol = u.Rol
        })
        .ToList();

    return Ok(safeUsers);
}


    // POST /users   → Crear usuario (solo admin)

    [Authorize(Roles = "admin")]
    [HttpPost]
    public IActionResult Create([FromBody] CreateUserDto dto)
    {
        var user = new Usuario
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            Rol = dto.Rol,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        safeUserDto safeUser = new safeUserDto
        {
            Id = user.Id,
            Nombre = user.Nombre,
            Email = user.Email,
            Rol = user.Rol
        };

        _context.Usuarios.Add(user);
        _context.SaveChanges();

        return Ok(safeUser);
    }


    // PUT /users/{id}  → Actualizar usuario (solo admin)

    [Authorize(Roles = "admin")]
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateUserDto dto)
    {
        var user = _context.Usuarios.Find(id);
        if (user == null) return NotFound();

        user.Nombre = dto.Nombre ?? user.Nombre;
        user.Email = dto.Email ?? user.Email;
        user.Rol = dto.Rol ?? user.Rol;

        if (!string.IsNullOrEmpty(dto.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        _context.SaveChanges();

        return Ok(user);
    }


    // DELETE /users/{id}  → Eliminar usuario (solo admin)

    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var user = _context.Usuarios.Find(id);
        if (user == null)
            return NotFound();

        _context.Usuarios.Remove(user);
        _context.SaveChanges();

        return Ok("Usuario eliminado");
    }
}

// DTOs
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