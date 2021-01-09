using System.Threading;
using System.Threading.Tasks;

namespace Redis.Master.Application
{
    public interface IMasterClient
    {
        Task SendReplicationRequestAsync(string slaveUrl, string key, string value, CancellationToken cancellationToken);
    }
}