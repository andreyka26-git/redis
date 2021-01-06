using System.Threading;
using System.Threading.Tasks;

namespace Redis.Master.Application
{
    public interface IChildClient
    {
        Task AddAsync(Child child, string key, uint hash, string value, CancellationToken cancellationToken);
        Task<string> GetAsync(Child child, string key, uint hash, CancellationToken cancellationToken);
    }
}