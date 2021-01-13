using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Redis.Master.Application;

namespace Redis.IntegrationTests.TestEntities
{
    public class FakeMasterClient : IMasterClient
    {
        public HttpClient HttpClient;

        public async Task SendReplicationRequestAsync(string slaveUrl, string key, string value, CancellationToken cancellationToken)
        {
            try
            {
                var url = $"{slaveUrl}/master/replication?key={key}";

                using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    var content = new StringContent(value, Encoding.UTF8, "application/json");
                    requestMessage.Content = content;

                    using (var response = await HttpClient.SendAsync(requestMessage, cancellationToken))
                    {
                        if (!response.IsSuccessStatusCode)
                            throw new Exception($"Cannot replicate, got status code: {response.StatusCode}. Body: {await response.Content.ReadAsStringAsync()}");
                    }
                }
            }
            catch 
            { 
                
            }
        }
    }
}
