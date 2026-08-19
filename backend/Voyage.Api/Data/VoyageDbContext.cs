using Microsoft.EntityFrameworkCore;
using Voyage.Api.Models;

namespace Voyage.Api.Data;

public class VoyageDbContext : DbContext
{
    //Recebe as configurações da ligação à base de dados.
    public VoyageDbContext(DbContextOptions<VoyageDbContext> options)
        : base(options)
    {
    }

    //Cada DbSet representa uma tabela da base de dados
    public DbSet<Usuario> Usuarios {get; set;} = null!;
    public DbSet<Roteiro> Roteiros {get; set;} = null!;
    public DbSet<Atividade> Atividades {get; set;} = null!; 

    // null! apenas informa ao C# que o Entity Framework preencherá estas propriedades quando a aplicação arrancar.
}