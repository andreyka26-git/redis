using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Redis.Child.Application;
using Redis.Child.Exceptions;
using Redis.Common.Abstractions;

namespace Redis.Child.Infrastructure
{
    public class Partition : IPartition
    {
        private readonly IPrimeNumberService _primeNumberService;

        private readonly LinkedList<Entry>[] _entries;
        private readonly int _entriesCount;
        private int _count;

        public Partition(IPrimeNumberService primeNumberService,
            IOptions<ChildOptions> options)
        {
            _primeNumberService = primeNumberService;
            _entriesCount = _primeNumberService.GetPrime(options.Value.PartitionItemsCount);

            _entries = new LinkedList<Entry>[_entriesCount];
            _count = 0;
        }

        public void Add<T>(string key, uint hashKey, T obj)
        {
            var jsonObj = JsonConvert.SerializeObject(obj);
            Add(key, hashKey, jsonObj);
        }
        
        public void Add(string key, uint hashKey, string obj)
        {
            if (_count >= _entriesCount)
                throw new ChildOverflowException();

            //try add new valuew with lightweight lock
            if (_entries[hashKey % _entriesCount] == null)
            {
                var newLinkedList = new LinkedList<Entry>();

                while(Interlocked.CompareExchange(ref _entries[hashKey % _entriesCount], newLinkedList, null) == null)
                {
                }
            }

            lock (_entries[hashKey % _entriesCount])
            {
                foreach (var entry in _entries[hashKey % _entriesCount])
                {
                    if (entry.Key == key)
                        throw new Exception("Object with the same value has already existed");
                }
                _entries[hashKey % _entriesCount].AddLast(new Entry(key, obj));
                _count++;
            }
        }

        public string Get(string key, uint hashKey)
        {
            var hashCodeList = _entries[hashKey % _entriesCount];

            foreach (var entry in hashCodeList)
            {
                if (entry.Key == key)
                    return entry.Value;
            }

            throw new Exception($"Cannot find node by {key} key");
        }

        public T Get<T>(string key, uint hashKey)
        {
            var hashCodeList = _entries[hashKey % _entriesCount];

            foreach (var entry in hashCodeList)
            {
                if (entry.Key == key)
                    return JsonConvert.DeserializeObject<T>(entry.Value);
            }

            throw new Exception($"Cannot find node by {key} key");
        }
    }
}
