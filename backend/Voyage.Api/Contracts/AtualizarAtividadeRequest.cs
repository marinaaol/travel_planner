using System.ComponentModel.DataAnnotations;

namespace Voyage.Api.Contracts;

public class AtualizarAtividadeRequest
{
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Tipo { get; set; } = string.Empty;

    public decimal? Valor { get; set; }

    public DateTime DataAtividade { get; set; }

    public TimeSpan? Hora { get; set; }

    [Range(1, int.MaxValue)]
    public int RoteiroId { get; set; }
}