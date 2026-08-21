using System.ComponentModel.DataAnnotations;

namespace Voyage.Api.Contracts;

public class AtualizarPerfilRequest
{
    // Novo nome apresentado na conta.
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    // Novo e-mail da conta.
    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    // É opcional: se não for enviada, a senha atual mantém-se.
    // Se for enviada, deve ter pelo menos 8 caracteres.
    [StringLength(100, MinimumLength = 8)]
    public string? NovaSenha { get; set; }
}