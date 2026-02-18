using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SK.Report.Models;

namespace SK.Report.Data.Mappings
{
    public class SessaoMap : IEntityTypeConfiguration<Models.Sessao>
    {
        public void Configure(EntityTypeBuilder<Sessao> builder)
        {
            //Tabela
            builder.ToTable("Sessao");

            // Chave Primária
            builder.HasKey(x => x.Ip);

            //Propriedades
            builder.Property(x => x.Ip).HasMaxLength(30);

            builder.Property(x => x.UserID).HasMaxLength(100);

            builder.Property(x => x.EditMode).HasMaxLength(10);

            builder.Property(x => x.WorkspaceID).HasMaxLength(100);

            builder.Property(x => x.ObjectType).HasMaxLength(30);

            builder.Property(x => x.ObjectID).HasMaxLength(100);

            builder.Property(x => x.Language).HasMaxLength(100);
        }
    }
}
