using System.Threading;
using System.Threading.Tasks;

namespace Redis.Master.Application
{
    public interface IMasterService
    {
        Task AddAsync(string key, string value, CancellationToken cancellationToken);
        Task<string> GetAsync(string key, CancellationToken cancellationToken);
    }
}