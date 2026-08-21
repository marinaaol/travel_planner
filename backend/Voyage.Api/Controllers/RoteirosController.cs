using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyage.Api.Contracts;
using Voyage.Api.Data;
using Voyage.Api.Models;

namespace Voyage.Api.Controllers;

// Exige um token JWT válido para usar qualquer rota deste controlador.
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RoteirosController : ControllerBase
{
    private readonly VoyageDbContext _context;

    public RoteirosController(VoyageDbContext context)
    {
        _context = context;
    }

    // Lê o identificador do utilizador que está dentro do token JWT.
    private int? ObterIdDoUtilizadorAutenticado()
    {
        var identificador = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(identificador, out var utilizadorId)
            ? utilizadorId
            : null;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos()
    {
        var utilizadorId = ObterIdDoUtilizadorAutenticado();

        if (utilizadorId is null)
        {
            return Unauthorized();
        }

        // Devolve somente os roteiros que pertencem ao utilizador autenticado.
        var roteiros = await _context.Roteiros
            .Where(roteiro => roteiro.UsuarioId == utilizadorId)
            .OrderBy(roteiro => roteiro.DataInicio)
            .Select(roteiro => new
            {
                roteiro.RoteiroId,
                roteiro.Titulo,
                roteiro.Destino,
                roteiro.DataInicio,
                roteiro.DataFim
            })
            .ToListAsync();

        return Ok(roteiros);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(
        [FromBody] CriarRoteiroRequest pedido)
    {
        var utilizadorId = ObterIdDoUtilizadorAutenticado();

        if (utilizadorId is null)
        {
            return Unauthorized();
        }

        if (pedido.DataFim < pedido.DataInicio)
        {
            return BadRequest(new
            {
                message = "A data de fim não pode ser anterior à data de início."
            });
        }

        // O dono do roteiro vem do token, não do pedido enviado pelo cliente.
        var roteiro = new Roteiro
        {
            Titulo = pedido.Titulo,
            Destino = pedido.Destino,
            DataInicio = pedido.DataInicio,
            DataFim = pedido.DataFim,
            UsuarioId = utilizadorId.Value
        };

        _context.Roteiros.Add(roteiro);
        await _context.SaveChangesAsync();

        return Created($"/api/roteiros/{roteiro.RoteiroId}", new
        {
            roteiro.RoteiroId,
            roteiro.Titulo,
            roteiro.Destino,
            roteiro.DataInicio,
            roteiro.DataFim
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(
        int id,
        [FromBody] AtualizarRoteiroRequest pedido)
    {
        var utilizadorId = ObterIdDoUtilizadorAutenticado();

        if (utilizadorId is null)
        {
            return Unauthorized();
        }

        if (pedido.DataFim < pedido.DataInicio)
        {
            return BadRequest(new
            {
                message = "A data de fim não pode ser anterior à data de início."
            });
        }

        // Procura o roteiro e confirma que ele pertence ao utilizador autenticado.
        var roteiro = await _context.Roteiros
            .FirstOrDefaultAsync(roteiro =>
                roteiro.RoteiroId == id &&
                roteiro.UsuarioId == utilizadorId.Value);

        if (roteiro is null)
        {
            return NotFound(new
            {
                message = "O roteiro indicado não existe."
            });
        }

        roteiro.Titulo = pedido.Titulo;
        roteiro.Destino = pedido.Destino;
        roteiro.DataInicio = pedido.DataInicio;
        roteiro.DataFim = pedido.DataFim;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            roteiro.RoteiroId,
            roteiro.Titulo,
            roteiro.Destino,
            roteiro.DataInicio,
            roteiro.DataFim
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Apagar(int id)
    {
        var utilizadorId = ObterIdDoUtilizadorAutenticado();

        if (utilizadorId is null)
        {
            return Unauthorized();
        }

        // Só pode apagar um roteiro que seja seu.
        var roteiro = await _context.Roteiros
            .FirstOrDefaultAsync(roteiro =>
                roteiro.RoteiroId == id &&
                roteiro.UsuarioId == utilizadorId.Value);

        if (roteiro is null)
        {
            return NotFound(new
            {
                message = "O roteiro indicado não existe."
            });
        }

        _context.Roteiros.Remove(roteiro);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}