namespace Voyage.Api.Models;

public class Usuario
{
    //corresponde à coluna "id" da tablea usuarios.
    public int Id {get; set;}

    //corresponde à coluna "nome".
    public string Nome {get; set;} = string.Empty;

    //corresponde à coluna "email".
    public string Email {get; set;} = string.Empty;

    //corresponde à coluna "senha_hash".
    //Nunca guarda a palavra-passe original.
    public string SenhaHash {get; set;} = string.Empty;

    //corresponde à coluna "criado_em".
    public DateTime CriadoEm {get; set;}    
}