using System.Threading;
using System.Threading.Tasks;

namespace Redis.Master.Application
{
    public interface IReplicationService
    {
        Task ReplicateToSlavesAsync(string key, string value, CancellationToken cancellationToken);
    }
}