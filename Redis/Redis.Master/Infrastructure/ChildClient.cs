using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Redis.Common.Dto;
using Redis.Master.Application;

namespace Redis.Master.Infrastructure
{
    public class ChildClient : IChildClient
    {
        private readonly HttpClient _httpClient;

        public ChildClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task AddAsync(Child child, string key, int hash, string value, CancellationToken cancellationToken)
        {
            var url = $"{child.ChildUrl}/partition";
            using (var message = new HttpRequestMessage(HttpMethod.Post, url))
            {
                var entry = new EntryDto
                {
                    HashKey = hash,
                    Key = key,
                    Value = value
                };

                var entryDto = JsonConvert.SerializeObject(entry);
                message.Content = new StringContent(entryDto, Encoding.UTF8, "application/json");

                using (var response = await _httpClient.SendAsync(message, cancellationToken))
                {
                    if (response.IsSuccessStatusCode)
                        throw new Exception($"StatusCode from child is not success. Status Code: {response.StatusCode}");
                }
            }
        }

        public async Task<string> GetAsync(Child child, string key, int hash, CancellationToken cancellationToken)
        {
            var url = $"{child.ChildUrl}/partition?key={key}&hashKey={hash}";
            using (var message = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (var response = await _httpClient.SendAsync(message, cancellationToken))
                {
                    if (response.IsSuccessStatusCode)
                        throw new Exception($"StatusCode from child is not success. Status Code: {response.StatusCode}");
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
            }
        }
    }
}
