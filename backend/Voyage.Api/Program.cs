using Microsoft.EntityFrameworkCore;
using Voyage.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// AddControllers prepara a API para receber pedidos organizados em controllers.
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("VoyageDatabase")
    ?? throw new InvalidOperationException(
        "A connection string 'VoyageDatabase' não foi encontrada.");

builder.Services.AddDbContext<VoyageDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

var app = builder.Build();

// Este endpoint é o primeiro teste: confirma que a API está viva antes do MySQL.
app.MapGet("/api/health", () => Results.Ok(new { message = "A API VOYAGE está a funcionar." }));
app.MapControllers();

app.Run();
