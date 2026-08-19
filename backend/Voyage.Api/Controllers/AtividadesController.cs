using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyage.Api.Data;

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
}