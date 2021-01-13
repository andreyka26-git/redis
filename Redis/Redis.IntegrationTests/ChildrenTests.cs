using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Redis.Child;
using Redis.Common.Dto;

namespace Redis.IntegrationTests
{
    [TestClass]
    public class ChildrenTests
    {
        private TestServer _server;

        [TestInitialize]
        public void Init()
        {
            var hostBuilder = new WebHostBuilder()
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("test.appsettings.json", optional: false)
                        .AddEnvironmentVariables();
                }).UseStartup<Startup>();

            _server = new TestServer(hostBuilder);
        }

        [TestMethod]
        public async Task Child_InsertValue_SeeItInsertedAsync()
        {
            var first = new EntryDto(105, "1", "value1");
            var second = new EntryDto(105, "2", "value2");
            var third = new EntryDto(106, "3", "value3");

            var data = new List<EntryDto>
            {
                first,
                second,
                third
            };

            using (var client = _server.CreateClient())
            {
                foreach (var entry in data)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(entry), Encoding.UTF8,
                        "application/json");

                    await client.PostAsync("/partition", content);
                }

                var response = await client.GetAsync("/partition/entries");
                var responseString = await response.Content.ReadAsStringAsync();
                var entries = JsonConvert.DeserializeObject<List<BucketDto>>(responseString);

                var firstBucket = entries.First(e => e.BucketIndex == 105);
                var secondBucket = entries.First(e => e.BucketIndex == 106);

                Assert.AreEqual(firstBucket.Entries.Count(), 2);
                Assert.AreEqual(secondBucket.Entries.Count(), 1);

                Assert.AreEqual(firstBucket.Entries.Last().Key, "2");
                Assert.AreEqual(firstBucket.Entries.Last().HashKey, (uint)105);
                Assert.AreEqual(firstBucket.Entries.Last().Value, "value2");

                Assert.AreEqual(firstBucket.Entries.First().Key, "1");
                Assert.AreEqual(firstBucket.Entries.First().HashKey, (uint)105);
                Assert.AreEqual(firstBucket.Entries.First().Value, "value1");

                Assert.AreEqual(secondBucket.Entries.Last().Key, "3");
                Assert.AreEqual(secondBucket.Entries.Last().HashKey, (uint)106);
                Assert.AreEqual(secondBucket.Entries.Last().Value, "value3");
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            _server.Dispose();
        }
    }
}
