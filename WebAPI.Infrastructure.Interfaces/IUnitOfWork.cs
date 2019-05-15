using System.Threading.Tasks;

namespace WebAPI.Infrastructure.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}