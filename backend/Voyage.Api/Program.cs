using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Voyage.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// CORS autoriza o front-end local (Live Server) a comunicar com esta API.
builder.Services.AddCors(opcoes =>
{
    opcoes.AddPolicy("FrontendLocal", politica =>
    {
        politica
            .WithOrigins(
                "http://localhost:5500",
                "http://127.0.0.1:5500",
                "http://127.0.0.1:5501"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Lê a configuração da ligação ao MySQL.
var connectionString = builder.Configuration.GetConnectionString("VoyageDatabase")
    ?? throw new InvalidOperationException(
        "A connection string 'VoyageDatabase' não foi encontrada."
    );

// Regista a ligação entre a API e a base de dados.
builder.Services.AddDbContext<VoyageDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

// Lê os dados JWT guardados em User Secrets.
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("A chave JWT não foi encontrada.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("O emissor JWT não foi encontrado.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("O público JWT não foi encontrado.");

// Define como a API confirma se um token recebido é verdadeiro e válido.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            // Não acrescenta minutos extra após a expiração.
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Aplica a autorização de origem definida acima aos pedidos recebidos.
app.UseCors("FrontendLocal");

// Lê e valida o token enviado em cada pedido protegido.
app.UseAuthentication();

// Aplica as regras de acesso definidas nos controladores.
app.UseAuthorization();

app.MapGet("/api/health", () => new
{
    message = "A API VOYAGE está a funcionar."
});

app.MapControllers();

app.Run();