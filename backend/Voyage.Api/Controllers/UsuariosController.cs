using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyage.Api.Data;

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
}