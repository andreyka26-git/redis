using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Redis.Common;
using Redis.Common.Abstractions;
using Redis.Common.Dto;
using Redis.Master.Application;

[assembly: InternalsVisibleTo("Redis.Tests")]
namespace Redis.Master.Infrastructure
{
    public class MasterService : IMasterService
    {
        private readonly IHashGenerator _hashGenerator;
        private readonly IChildClient _client;
        private readonly IPrimeNumberService _primeNumberService;
        private readonly MasterOptions _options;
        
        private readonly int _overallCount;
        private readonly List<Child> _children;

        public MasterService(IHashGenerator hashGenerator,
            IChildClient client,
            IPrimeNumberService primeNumberService,
            IConfiguration config,
            IOptions<MasterOptions> options)
        {
            _options = options.Value;
            _hashGenerator = hashGenerator;
            _client = client;

            var partitionItemsCount =  primeNumberService.GetPrime(config.GetValue<int>(GlobalConsts.PartitionItemsCountName));

            _overallCount = partitionItemsCount * _options.Children.Count;
            _children = InitializeChildren(_options.Children, partitionItemsCount);
        }

        public async Task AddAsync(string key, string value, CancellationToken cancellationToken)
        {
            var hash = _hashGenerator.GenerateHash(key);
            var child = DetermineChildByHash(_children, key, hash, _overallCount);

            await _client.AddAsync(child, key, hash, value, cancellationToken);
        }

        public async Task<List<ChildEntriesDto>> GetAllEntriesAsync(CancellationToken cancellationToken)
        {
            var resp = new List<ChildEntriesDto>();

            var tasks = new List<(string url, Task<List<BucketDto>> bucketsTask)>();

            foreach (var child in _children)
                tasks.Add((child.ChildUrl, Task.Run(() => _client.GetAllEntriesAsync(child, cancellationToken), cancellationToken)));

            foreach (var task in tasks)
            {
                var buckets = await task.bucketsTask;
                resp.Add(new ChildEntriesDto(task.url, buckets));
            }

            return resp;
        }

        public async Task<string> GetAsync(string key, CancellationToken cancellationToken)
        {
            var hash = _hashGenerator.GenerateHash(key);
            var child = DetermineChildByHash(_children, key, hash, _overallCount);

            var value = await _client.GetAsync(child, key, hash, cancellationToken);
            return value;
        }

        internal Child DetermineChildByHash(List<Child> children, string key, uint hash, int overallCount)
        {
            //TODO think about not O(n) solution for that, use binary search at least to get O(log n)
            var hashMod = hash % overallCount;

            foreach (var child in children)
            {
                if (hashMod >= child.MinHash && hashMod <= child.MaxHash)
                    return child;
            }

            throw new Exception($"Cannot determine child node for this {hash} hash and {key} key.");
        }

        internal List<Child> InitializeChildren(List<string> childrenUrls, int partitionItemsCount)
        {
            var children = new List<Child>(childrenUrls.Count);
            var rangeMultiplier = 1;

            foreach (var childUrl in childrenUrls)
            {
                var max = rangeMultiplier * partitionItemsCount - 1;
                var min = max - partitionItemsCount + 1;

                var child = new Child(childUrl, min, max);
                children.Add(child);
                rangeMultiplier++;
            }

            return children;
        }
    }
}
