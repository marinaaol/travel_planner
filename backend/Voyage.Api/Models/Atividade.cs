namespace Voyage.Api.Models;

public class Atividade
{
    //corresponde à coluna "AtividadeId"
    public int AtividadeId {get; set;}

    //corresponde à coluna "Nome".
    public string Nome {get; set;} = string.Empty;

    //corresponde à coluna "Tipo".
    public string Tipo {get; set;} = string.Empty;

    //corresponde à coluna "Valor".
    public decimal? Valor {get; set;}
    
    //corresponde à coluna "DataAtividade".
    public DateTime DataAtividade {get; set;}

    //corresponde à coluna "Hora".
    public TimeSpan? Hora {get; set;}

    //corresponde à coluna "RoteiroId".
    public int RoteiroId {get; set;}
}