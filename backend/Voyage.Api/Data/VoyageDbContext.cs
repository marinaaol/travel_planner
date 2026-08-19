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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Usuario>(entity =>
    {
        entity.ToTable("usuarios");

        entity.HasKey(usuario => usuario.Id);

        entity.Property(usuario => usuario.Id)
            .HasColumnName("id");

        entity.Property(usuario => usuario.Nome)
            .HasColumnName("nome");

        entity.Property(usuario => usuario.Email)
            .HasColumnName("email");

        entity.Property(usuario => usuario.SenhaHash)
            .HasColumnName("senha_hash");

        entity.Property(usuario => usuario.CriadoEm)
            .HasColumnName("criado_em");
    });

    modelBuilder.Entity<Roteiro>(entity =>
    {
        entity.ToTable("roteiros");

        entity.HasKey(roteiro => roteiro.RoteiroId);

        entity.Property(roteiro => roteiro.RoteiroId)
            .HasColumnName("roteiro_id");

        entity.Property(roteiro => roteiro.Titulo)
            .HasColumnName("titulo");

        entity.Property(roteiro => roteiro.Destino)
            .HasColumnName("destino");

        entity.Property(roteiro => roteiro.DataInicio)
            .HasColumnName("data_inicio");

        entity.Property(roteiro => roteiro.DataFim)
            .HasColumnName("data_fim");

        entity.Property(roteiro => roteiro.UsuarioId)
            .HasColumnName("usuario_id");
    });


}
}