using ApiUsuarios.Data;
using ApiUsuarios.Models;
using ApiUsuarios.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // Correcto
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// ----------------------------------
// 1. Configurar EF Core + PostgreSQL
// ----------------------------------
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// ----------------------------------
// 2. Servicios necesarios
// ----------------------------------
builder.Services.AddSingleton<JwtService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----------------------------------
// 3. Autenticación JWT
// ----------------------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"]!)
            )
        };
    });

// ----------------------------------
// 4. Autorización
// ----------------------------------
builder.Services.AddAuthorization();

// ----------------------------------
// Construir la app
// ----------------------------------
var app = builder.Build();

// ----------------------------------
// 5. Migrar BD y crear admin inicial
// ----------------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // Crear admin solo si no existe ninguno
    if (!db.Usuarios.Any())
    {
        db.Usuarios.Add(new Usuario
        {
            Nombre = "SuperAdmin",
            Email = "admin@admin.com",
            Rol = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123")
        });

        db.SaveChanges();
        Console.WriteLine("⚡ Usuario administrador inicial creado: admin@admin.com / admin123");
    }
}

// ----------------------------------
// 6. Middleware
// ----------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
