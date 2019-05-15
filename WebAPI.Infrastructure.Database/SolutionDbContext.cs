using Microsoft.EntityFrameworkCore;
using WebAPI.Infrastructure.Database.EntityConfiguration;
using WebAPI.Infrastructure.DomainModel;

namespace WebAPI.Infrastructure.Database
{
    public class SolutionDbContext:DbContext
    {
        public SolutionDbContext(DbContextOptions<SolutionDbContext> options):base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
        }

        public DbSet<Order> Orders { get; set; }
    }
}