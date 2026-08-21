using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Voyage.Api.Contracts;
using Voyage.Api.Data;
using Voyage.Api.Models;

namespace Voyage.Api.Controllers;

// Por padrão, as rotas deste controlador exigem token JWT.
[Authorize]
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

    // Obtém o id da conta presente no token JWT.
    private int? ObterIdDoUtilizadorAutenticado()
    {
        var identificador = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(identificador, out var utilizadorId)
            ? utilizadorId
            : null;
    }

    [AllowAnonymous]
    [HttpPost("registo")]
    public async Task<IActionResult> Registar(
        [FromBody] RegistarUtilizadorRequest pedido)
    {
        var emailNormalizado = pedido.Email.Trim().ToLowerInvariant();

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

        // Guarda apenas o hash, nunca a senha original.
        var hasher = new PasswordHasher<Usuario>();
        utilizador.SenhaHash = hasher.HashPassword(utilizador, pedido.Senha);

        _context.Usuarios.Add(utilizador);
        await _context.SaveChangesAsync();

        return Created($"/api/usuarios/perfil", new
        {
            utilizador.Id,
            utilizador.Nome,
            utilizador.Email,
            utilizador.CriadoEm
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest pedido)
    {
        var emailNormalizado = pedido.Email.Trim().ToLowerInvariant();

        var utilizador = await _context.Usuarios
            .FirstOrDefaultAsync(usuario => usuario.Email == emailNormalizado);

        // Não revela se o e-mail existe ou não.
        if (utilizador is null)
        {
            return Unauthorized(new
            {
                message = "E-mail ou palavra-passe inválidos."
            });
        }

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

        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("A chave JWT não foi encontrada.");

        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];

        var expiraEmMinutos = int.Parse(
            _configuration["Jwt:ExpiresInMinutes"] ?? "60"
        );

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, utilizador.Id.ToString()),
            new(ClaimTypes.Name, utilizador.Nome),
            new(ClaimTypes.Email, utilizador.Email)
        };

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

    [HttpGet("perfil")]
    public async Task<IActionResult> ObterPerfil()
    {
        var utilizadorId = ObterIdDoUtilizadorAutenticado();

        if (utilizadorId is null)
        {
            return Unauthorized();
        }

        var utilizador = await _context.Usuarios.FindAsync(utilizadorId.Value);

        if (utilizador is null)
        {
            return NotFound(new
            {
                message = "O utilizador indicado não existe."
            });
        }

        // Nunca devolvemos o hash da senha.
        return Ok(new
        {
            utilizador.Id,
            utilizador.Nome,
            utilizador.Email,
            utilizador.CriadoEm
        });
    }

    [HttpPut("perfil")]
    public async Task<IActionResult> AtualizarPerfil(
        [FromBody] AtualizarPerfilRequest pedido)
    {
        var utilizadorId = ObterIdDoUtilizadorAutenticado();

        if (utilizadorId is null)
        {
            return Unauthorized();
        }

        var utilizador = await _context.Usuarios.FindAsync(utilizadorId.Value);

        if (utilizador is null)
        {
            return NotFound(new
            {
                message = "O utilizador indicado não existe."
            });
        }

        var emailNormalizado = pedido.Email.Trim().ToLowerInvariant();

        // Verifica se o novo e-mail pertence a outra conta.
        var emailJaExiste = await _context.Usuarios.AnyAsync(usuario =>
            usuario.Email == emailNormalizado &&
            usuario.Id != utilizadorId.Value);

        if (emailJaExiste)
        {
            return Conflict(new
            {
                message = "Já existe uma conta associada a este e-mail."
            });
        }

        utilizador.Nome = pedido.Nome.Trim();
        utilizador.Email = emailNormalizado;

        // Só altera a senha se uma nova senha tiver sido enviada.
        if (!string.IsNullOrWhiteSpace(pedido.NovaSenha))
        {
            var hasher = new PasswordHasher<Usuario>();
            utilizador.SenhaHash = hasher.HashPassword(
                utilizador,
                pedido.NovaSenha
            );
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            utilizador.Id,
            utilizador.Nome,
            utilizador.Email,
            utilizador.CriadoEm
        });
    }

    [HttpDelete("perfil")]
    public async Task<IActionResult> ApagarPerfil()
    {
        var utilizadorId = ObterIdDoUtilizadorAutenticado();

        if (utilizadorId is null)
        {
            return Unauthorized();
        }

        var utilizador = await _context.Usuarios.FindAsync(utilizadorId.Value);

        if (utilizador is null)
        {
            return NotFound(new
            {
                message = "O utilizador indicado não existe."
            });
        }

        // O CASCADE do MySQL apaga também roteiros e atividades desta conta.
        _context.Usuarios.Remove(utilizador);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "A conta, os seus roteiros e as suas atividades foram eliminados permanentemente."
        });
    }
}