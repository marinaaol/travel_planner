using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyage.Api.Contracts;
using Voyage.Api.Data;
using Voyage.Api.Models;

namespace Voyage.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoteirosController : ControllerBase
{
    private readonly VoyageDbContext _context;

    public RoteirosController(VoyageDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos()
    {
        var roteiros = await _context.Roteiros
            .OrderBy(roteiro => roteiro.DataInicio)
            .Select(roteiro => new
            {
                roteiro.RoteiroId,
                roteiro.Titulo,
                roteiro.Destino,
                roteiro.DataInicio,
                roteiro.DataFim,
                roteiro.UsuarioId
            })
            .ToListAsync();

        return Ok(roteiros);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarRoteiroRequest pedido)
    {
        if (pedido.DataFim < pedido.DataInicio)
        {
            return BadRequest(new
            {
                message = "A data de fim não pode ser anterior à data de início."
            });
        }

        var usuarioExiste = await _context.Usuarios
            .AnyAsync(usuario => usuario.Id == pedido.UsuarioId);

        if (!usuarioExiste)
        {
            return BadRequest(new
            {
                message = "O utilizador indicado não existe."
            });
        }

        var roteiro = new Roteiro
        {
            Titulo = pedido.Titulo,
            Destino = pedido.Destino,
            DataInicio = pedido.DataInicio,
            DataFim = pedido.DataFim,
            UsuarioId = pedido.UsuarioId
        };

        _context.Roteiros.Add(roteiro);
        await _context.SaveChangesAsync();

        return Created($"/api/roteiros/{roteiro.RoteiroId}", new
        {
            roteiro.RoteiroId,
            roteiro.Titulo,
            roteiro.Destino,
            roteiro.DataInicio,
            roteiro.DataFim,
            roteiro.UsuarioId
        });
    }
}