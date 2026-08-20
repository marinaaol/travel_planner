using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyage.Api.Data;
using Voyage.Api.Contracts;
using Voyage.Api.Models;

namespace Voyage.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AtividadesController : ControllerBase
{
    private readonly VoyageDbContext _context;

    public AtividadesController(VoyageDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos()
    {
        var atividades = await _context.Atividades
            .OrderBy(atividade => atividade.DataAtividade)
            .ThenBy(atividade => atividade.Hora) //ThenBy é o segundo critério de organização: primeiro por dia e, quando existirem atividades no mesmo dia, por hora — como numa agenda.
            .Select(atividade => new
            {
                atividade.AtividadeId,
                atividade.Nome,
                atividade.Tipo,
                atividade.Valor,
                atividade.DataAtividade,
                atividade.Hora,
                atividade.RoteiroId
            })
            .ToListAsync();

        return Ok(atividades);
    }
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarAtividadeRequest pedido)
    {
        if (pedido.Valor is < 0)
        {
            return BadRequest(new
            {
                message = "O valor da atividade não pode ser negativo."
            });
        }

        var roteiro = await _context.Roteiros
            .FirstOrDefaultAsync(item => item.RoteiroId == pedido.RoteiroId);

        if (roteiro is null)
        {
            return BadRequest(new
            {
                message = "O roteiro indicado não existe."
            });
        }

        if (pedido.DataAtividade.Date < roteiro.DataInicio.Date ||
            pedido.DataAtividade.Date > roteiro.DataFim.Date)
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
        // Impede que uma atividade seja gravada com valor negativo.
        if (pedido.Valor is < 0)
        {
            return BadRequest(new
            {
                message = "O valor da atividade não pode ser negativo."
            });
        }
        // Procura a atividade existente pelo id recebido na URL.
        var atividade = await _context.Atividades.FindAsync(id);

        if (atividade is null)
        {
            return NotFound(new
            {
                message = "A atividade indicada não existe."
            });
        }
        // Confirma que o novo roteiro indicado realmente existe.
        var roteiro = await _context.Roteiros.FindAsync(pedido.RoteiroId);

        if (roteiro is null)
        {
            return BadRequest(new
            {
                message = "O roteiro indicado não existe."
            });
        }
        // Garante que a data da atividade cabe dentro das datas do novo roteiro.
        if (pedido.DataAtividade < roteiro.DataInicio ||
            pedido.DataAtividade > roteiro.DataFim)
        {
            return BadRequest(new
            {
                message = "A data da atividade deve estar dentro do período do roteiro."
            });
        }
        // Atualiza os dados da atividade encontrada.
        atividade.Nome = pedido.Nome;
        atividade.Tipo = pedido.Tipo;
        atividade.Valor = pedido.Valor;
        atividade.DataAtividade = pedido.DataAtividade;
        atividade.Hora = pedido.Hora;
        atividade.RoteiroId = pedido.RoteiroId;

        // Guarda definitivamente as alterações no MySQL.
        await _context.SaveChangesAsync();

        // Devolve a atividade atualizada com HTTP 200 OK.
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
}