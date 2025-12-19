using ApiUsuarios.Data;
using ApiUsuarios.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------
// 1. EF Core + PostgreSQL
// ----------------------------------
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// ----------------------------------
// 2. Controladores
// ----------------------------------
builder.Services.AddControllers();

// ----------------------------------
// 3. Swagger
// ----------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----------------------------------
// 4. CORS(configurar los dominios permitidos)
// ----------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ----------------------------------
// 5. Servicios
// ----------------------------------
builder.Services.AddSingleton<JwtService>();

// ----------------------------------
// 6. Autenticación JWT
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
// 7. Autorización
// ----------------------------------
builder.Services.AddAuthorization();

// ----------------------------------
// 8. Construir la app
// ----------------------------------
var app = builder.Build();

// ----------------------------------
// 9. Middleware
// ----------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS SIEMPRE antes de auth
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
