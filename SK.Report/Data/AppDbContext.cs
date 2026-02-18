using Microsoft.EntityFrameworkCore;
using SK.Report.Data.Mappings;

namespace SK.Report.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        { }

        public DbSet<Models.Sessao> Sessoes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SessaoMap());
        }
    }
}
