using System.ComponentModel.DataAnnotations;

namespace Voyage.Api.Contracts;

public class CriarRoteiroRequest
{
    [Required]
    [StringLength(250)]
    public string Titulo {get; set;} = string.Empty;

    [Required]
    [StringLength(250)]
    public string Destino {get; set;} = string.Empty;

    public DateTime DataInicio {get;set;}
    public DateTime DataFim {get; set;}
    
    [Range(1, int.MaxValue)]
    public int UsuarioId {get; set;}
    
}