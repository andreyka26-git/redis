

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Redis.Master.Application;

namespace Redis.Master.Infrastructure
{
    public class ReplicationService : IReplicationService
    {
        private readonly IMasterClient _client;
        private readonly MasterOptions _options;

        public ReplicationService(IMasterClient client, 
            IOptions<MasterOptions> options)
        {
            _options = options.Value;
            _client = client;
        }

        public Task ReplicateToSlavesAsync(string key, string value, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            foreach (var slaveUrl in _options.Slaves)
            {
                tasks.Add(_client.SendReplicationRequestAsync(slaveUrl, key, value, cancellationToken));
            }

            return Task.WhenAll(tasks);
        }
    }
}
