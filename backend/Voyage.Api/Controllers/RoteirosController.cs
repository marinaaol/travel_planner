using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Voyage.Api.Data;

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
            .OrderBy(roteiro => roteiro.DataInicio) //OrderBy: organiza os roteiros por data de início, como uma agenda de viagem em ordem cronológica.
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
}