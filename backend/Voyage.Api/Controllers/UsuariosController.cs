using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyage.Api.Data;
using Microsoft.AspNetCore.Identity;
using Voyage.Api.Contracts;
using Voyage.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Voyage.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly VoyageDbContext _context;
    private readonly IConfiguration _configuration;

    public UsuariosController(
    VoyageDbContext context,
    IConfiguration configuration)
{
    _context = context;
    _configuration = configuration;
}

    [HttpGet]
    public async Task<IActionResult> ObterTodos()
    {
        var usuarios = await _context.Usuarios
            .Select(usuario => new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                usuario.CriadoEm
            })
            .ToListAsync();

        return Ok(usuarios);
    }
    [HttpPost("registo")]
    public async Task<IActionResult> Registar(
        [FromBody] RegistarUtilizadorRequest pedido)
    {
        // Remove espaços acidentais e mantém o e-mail num formato consistente.
        var emailNormalizado = pedido.Email.Trim().ToLowerInvariant();

        // Impede o registo de duas contas com o mesmo e-mail.
        var emailJaExiste = await _context.Usuarios
            .AnyAsync(usuario => usuario.Email == emailNormalizado);

        if (emailJaExiste)
        {
            return Conflict(new
            {
                message = "Já existe uma conta associada a este e-mail."
            });
        }

        var utilizador = new Usuario
        {
            Nome = pedido.Nome.Trim(),
            Email = emailNormalizado
        };

        // Converte a palavra-passe num hash antes de a guardar.
        // O texto original da senha não entra na base de dados.
        var hasher = new PasswordHasher<Usuario>();
        utilizador.SenhaHash = hasher.HashPassword(utilizador, pedido.Senha);

        _context.Usuarios.Add(utilizador);
        await _context.SaveChangesAsync();

        return Created($"/api/usuarios/{utilizador.Id}", new
        {
            utilizador.Id,
            utilizador.Nome,
            utilizador.Email,
            utilizador.CriadoEm
        });
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest pedido)
    {
        // Normaliza o e-mail para procurar a conta de forma consistente.
        var emailNormalizado = pedido.Email.Trim().ToLowerInvariant();

        var utilizador = await _context.Usuarios
            .FirstOrDefaultAsync(usuario => usuario.Email == emailNormalizado);

        // A mesma mensagem evita revelar se o e-mail existe ou não.
        if (utilizador is null)
        {
            return Unauthorized(new
            {
                message = "E-mail ou palavra-passe inválidos."
            });
        }

        // Compara a senha enviada com o hash guardado no MySQL.
        var hasher = new PasswordHasher<Usuario>();
        var resultado = hasher.VerifyHashedPassword(
            utilizador,
            utilizador.SenhaHash,
            pedido.Senha
        );

        if (resultado == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new
            {
                message = "E-mail ou palavra-passe inválidos."
            });
        }

        // Obtém a chave e os dados que serão gravados no token.
        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("A chave JWT não foi encontrada.");

        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];

        var expiraEmMinutos = int.Parse(
            _configuration["Jwt:ExpiresInMinutes"] ?? "60"
        );

        // As claims são os dados de identidade dentro do token.
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, utilizador.Id.ToString()),
            new(ClaimTypes.Name, utilizador.Nome),
            new(ClaimTypes.Email, utilizador.Email)
        };

        // Assina o token com a chave secreta.
        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiraEmMinutos),
            signingCredentials: credenciais
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiraEm = token.ValidTo,
            utilizador = new
            {
                utilizador.Id,
                utilizador.Nome,
                utilizador.Email
            }
        });
    }
}