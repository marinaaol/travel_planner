using System.ComponentModel.DataAnnotations;

namespace Voyage.Api.Contracts;

public class RegistarUtilizadorRequest
{
    // Nome apresentado na conta do utilizador.
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    // Confirma que o texto tem formato de e-mail.
    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    // A palavra-passe chega à API apenas para ser transformada em hash.
    // Nunca será guardada diretamente na base de dados.
    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Senha { get; set; } = string.Empty;
}