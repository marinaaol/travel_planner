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
}