using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApiUsuarios.Data;
using ApiUsuarios.Models;
using BCrypt.Net;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionResult> Me()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
            return Unauthorized("No se encontró el claim NameIdentifier en el token.");

        int userId = int.Parse(claim.Value);

        var user = await _context.Usuarios.FindAsync(userId);
        if (user == null)
            return NotFound("Usuario no encontrado.");

        var safeUser = new safeUserDto
        {
            Id = user.Id,
            Nombre = user.Nombre,
            Email = user.Email,
            Rol = user.Rol
        };

        return Ok(safeUser);
    }

    // GET /users   → Solo admin puede ver a todos los usuarios
    [Authorize(Roles = "admin,subadmin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var safeUsers = await _context.Usuarios
            .Select(u => new safeUserDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Email = u.Email,
                Rol = u.Rol
            })
            .ToListAsync();

        return Ok(safeUsers);
    }

    // POST /users   → Crear usuario (solo admin)
    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var user = new Usuario
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            Rol = dto.Rol,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Usuarios.Add(user);
        await _context.SaveChangesAsync();

        var safeUser = new safeUserDto
        {
            Id = user.Id,
            Nombre = user.Nombre,
            Email = user.Email,
            Rol = user.Rol
        };

        return Ok(safeUser);
    }

    // PUT /users/{id}  → Actualizar usuario (solo admin)
    [Authorize(Roles = "admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        var user = await _context.Usuarios.FindAsync(id);
        if (user == null)
            return NotFound();

        user.Nombre = dto.Nombre ?? user.Nombre;
        user.Email = dto.Email ?? user.Email;
        user.Rol = dto.Rol ?? user.Rol;

        if (!string.IsNullOrEmpty(dto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        await _context.SaveChangesAsync();

        var safeUser = new safeUserDto
        {
            Id = user.Id,
            Nombre = user.Nombre,
            Email = user.Email,
            Rol = user.Rol
        };

        return Ok(safeUser);
    }

    // DELETE /users/{id}  → Eliminar usuario (solo admin)
    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Usuarios.FindAsync(id);
        if (user == null)
            return NotFound();

        _context.Usuarios.Remove(user);
        await _context.SaveChangesAsync();

        return Ok("Usuario eliminado");
    }
}
