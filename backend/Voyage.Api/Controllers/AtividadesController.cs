using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyage.Api.Contracts;
using Voyage.Api.Data;
using Voyage.Api.Models;

namespace Voyage.Api.Controllers;

// Exige token JWT para todas as operações com atividades.
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AtividadesController : ControllerBase
{
    private readonly VoyageDbContext _context;

    public AtividadesController(VoyageDbContext context)
    {
        _context = context;
    }

    // Obtém o id do utilizador guardado dentro do token JWT.
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

        // Junta atividades e roteiros para mostrar somente atividades
        // de roteiros pertencentes ao utilizador autenticado.
        var atividades = await (
            from atividade in _context.Atividades
            join roteiro in _context.Roteiros
                on atividade.RoteiroId equals roteiro.RoteiroId
            where roteiro.UsuarioId == utilizadorId.Value
            orderby atividade.DataAtividade, atividade.Hora
            select new
            {
                atividade.AtividadeId,
                atividade.Nome,
                atividade.Tipo,
                atividade.Valor,
                atividade.DataAtividade,
                atividade.Hora,
                atividade.RoteiroId
            }
        ).ToListAsync();

        return Ok(atividades);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(
        [FromBody] CriarAtividadeRequest pedido)
    {
        var utilizadorId = ObterIdDoUtilizadorAutenticado();

        if (utilizadorId is null)
        {
            return Unauthorized();
        }

        if (pedido.Valor is < 0)
        {
            return BadRequest(new
            {
                message = "O valor da atividade não pode ser negativo."
            });
        }

        // Confirma que o roteiro existe e pertence ao utilizador do token.
        var roteiro = await _context.Roteiros
            .FirstOrDefaultAsync(roteiro =>
                roteiro.RoteiroId == pedido.RoteiroId &&
                roteiro.UsuarioId == utilizadorId.Value);

        if (roteiro is null)
        {
            return NotFound(new
            {
                message = "O roteiro indicado não existe."
            });
        }

        if (pedido.DataAtividade < roteiro.DataInicio ||
            pedido.DataAtividade > roteiro.DataFim)
        {
            return BadRequest(new
            {
                message = "A data da atividade deve estar dentro do período do roteiro."
            });
        }

        var atividade = new Atividade
        {
            Nome = pedido.Nome,
            Tipo = pedido.Tipo,
            Valor = pedido.Valor,
            DataAtividade = pedido.DataAtividade,
            Hora = pedido.Hora,
            RoteiroId = pedido.RoteiroId
        };

        _context.Atividades.Add(atividade);
        await _context.SaveChangesAsync();

        return Created($"/api/atividades/{atividade.AtividadeId}", new
        {
            atividade.AtividadeId,
            atividade.Nome,
            atividade.Tipo,
            atividade.Valor,
            atividade.DataAtividade,
            atividade.Hora,
            atividade.RoteiroId
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(
        int id,
        [FromBody] AtualizarAtividadeRequest pedido)
    {
        var utilizadorId = ObterIdDoUtilizadorAutenticado();

        if (utilizadorId is null)
        {
            return Unauthorized();
        }

        if (pedido.Valor is < 0)
        {
            return BadRequest(new
            {
                message = "O valor da atividade não pode ser negativo."
            });
        }

        // Procura a atividade e confirma que o respetivo roteiro é do utilizador.
        var atividade = await (
            from item in _context.Atividades
            join roteiro in _context.Roteiros
                on item.RoteiroId equals roteiro.RoteiroId
            where item.AtividadeId == id &&
                  roteiro.UsuarioId == utilizadorId.Value
            select item
        ).FirstOrDefaultAsync();

        if (atividade is null)
        {
            return NotFound(new
            {
                message = "A atividade indicada não existe."
            });
        }

        // Confirma que o novo roteiro também pertence ao utilizador.
        var novoRoteiro = await _context.Roteiros
            .FirstOrDefaultAsync(roteiro =>
                roteiro.RoteiroId == pedido.RoteiroId &&
                roteiro.UsuarioId == utilizadorId.Value);

        if (novoRoteiro is null)
        {
            return NotFound(new
            {
                message = "O roteiro indicado não existe."
            });
        }

        if (pedido.DataAtividade < novoRoteiro.DataInicio ||
            pedido.DataAtividade > novoRoteiro.DataFim)
        {
            return BadRequest(new
            {
                message = "A data da atividade deve estar dentro do período do roteiro."
            });
        }

        atividade.Nome = pedido.Nome;
        atividade.Tipo = pedido.Tipo;
        atividade.Valor = pedido.Valor;
        atividade.DataAtividade = pedido.DataAtividade;
        atividade.Hora = pedido.Hora;
        atividade.RoteiroId = pedido.RoteiroId;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            atividade.AtividadeId,
            atividade.Nome,
            atividade.Tipo,
            atividade.Valor,
            atividade.DataAtividade,
            atividade.Hora,
            atividade.RoteiroId
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

        // Só encontra a atividade se ela estiver num roteiro do utilizador atual.
        var atividade = await (
            from item in _context.Atividades
            join roteiro in _context.Roteiros
                on item.RoteiroId equals roteiro.RoteiroId
            where item.AtividadeId == id &&
                  roteiro.UsuarioId == utilizadorId.Value
            select item
        ).FirstOrDefaultAsync();

        if (atividade is null)
        {
            return NotFound(new
            {
                message = "A atividade indicada não existe."
            });
        }

        _context.Atividades.Remove(atividade);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}