using System.ComponentModel.DataAnnotations;

namespace Voyage.Api.Contracts;

public class LoginRequest
{
    // Identifica a conta que tenta entrar.
    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    // É usada apenas para confirmar a identidade.
    // Nunca será devolvida pela API nem gravada em texto simples.
    [Required]
    public string Senha { get; set; } = string.Empty;
}