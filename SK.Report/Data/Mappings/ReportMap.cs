using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SK.Report.Data.Mappings
{
    public class ReportMap : IEntityTypeConfiguration<Models.Report>
    {
        public void Configure(EntityTypeBuilder<Models.Report> builder)
        {
            //Tabela
            builder.ToTable("report");

            // Chave Primária
            builder.HasKey(x => x.Id);

            // Identity
            builder.HasKey(x => x.Id);

            //Propriedades
            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("varying(36)");

            builder.Property(x => x.Title)
                .HasColumnName("title")
                .HasColumnType("varying(250)");

            builder.Property(x => x.Content)
                .HasColumnName("content")
                .HasColumnType("text");

            builder.Property(x => x.ReportType)
                .HasColumnName("reporttype")
                .HasColumnType("varying(10)");

            builder.Property(x => x.App)
                .HasColumnName("app")
                .HasColumnType("varying(12)");

            builder.Property(x => x.IsValid)
                .HasColumnName("isvalid")
                .HasColumnType("boolean");
        }
    }
}
