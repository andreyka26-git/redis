using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Redis.Common.Dto;

namespace Redis.Master.Application
{
    public interface IChildClient
    {
        Task<List<BucketDto>> GetAllEntriesAsync(Child child, CancellationToken cancellationToken);
        Task AddAsync(Child child, string key, uint hash, string value, CancellationToken cancellationToken);
        Task<string> GetAsync(Child child, string key, uint hash, CancellationToken cancellationToken);
    }
}