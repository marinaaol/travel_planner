namespace Voyage.Api.Models;

public class Roteiro
{
    //corresponde à coluna "RoteiroId"
    public int RoteiroId {get; set;}

    //corresponde à coluna "Titulo".
    public string Titulo {get; set;} = string.Empty;
    
    //corresponde à coluna "Destino".
    public string Destino {get; set;} = string.Empty;

    //corresponde à coluna "DataInicio".
    public DateTime DataInicio {get; set;}

    //corresponde à coluna "DataFim".
    public DateTime DataFim {get; set;}

    //corresponde à coluna "UsuarioId".
    public int UsuarioId {get; set;}
    
}