using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyage.Api.Data;
using Microsoft.AspNetCore.Identity;
using Voyage.Api.Contracts;
using Voyage.Api.Models;

namespace Voyage.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly VoyageDbContext _context;

    public UsuariosController(VoyageDbContext context)
    {
        _context = context;
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
}