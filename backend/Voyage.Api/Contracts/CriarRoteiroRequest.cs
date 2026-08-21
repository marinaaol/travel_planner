using System.ComponentModel.DataAnnotations;

namespace Voyage.Api.Contracts;

public class CriarRoteiroRequest
{
    [Required]
    [StringLength(250)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    public string Destino { get; set; } = string.Empty;

    public DateTime DataInicio { get; set; }

    public DateTime DataFim { get; set; }

    // UsuarioId foi removido.
    // O utilizador será identificado pelo token JWT.
}