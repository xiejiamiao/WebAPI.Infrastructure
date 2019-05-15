using System.Threading.Tasks;
using WebAPI.Infrastructure.Database;
using WebAPI.Infrastructure.Interfaces;

namespace WebAPI.Infrastructure.Repositories
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly SolutionDbContext _dbContext;

        public UnitOfWork(SolutionDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}