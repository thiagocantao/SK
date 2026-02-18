using Microsoft.EntityFrameworkCore;
using SK.Report.Data.Mappings;

namespace SK.Report.Data
{
    public class ReportContext : DbContext
    {
        public ReportContext(DbContextOptions<ReportContext> options)
            : base(options)
        {

        }

        public DbSet<Models.Report> Reports { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ReportMap());
        }
    }
}
