using Microsoft.EntityFrameworkCore;
using docker_dotnet_three_js.DataAccess.Implementations.Entities;

namespace docker_dotnet_three_js.DataAccess.DBContexts
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<FileElement>? FileElement { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileElement>(entity =>
            {
                entity.Property(e => e.Id).UseIdentityColumn();
            });
        }
    }
}
