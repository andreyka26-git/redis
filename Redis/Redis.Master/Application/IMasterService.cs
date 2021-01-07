using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Redis.Common.Dto;

namespace Redis.Master.Application
{
    public interface IMasterService
    {
        Task<List<ChildEntriesDto>> GetAllEntriesAsync(CancellationToken cancellationToken);
        Task AddAsync(string key, string value, CancellationToken cancellationToken);
        Task<string> GetAsync(string key, CancellationToken cancellationToken);
    }
}