using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Redis.Child;
using Redis.Common.Abstractions;
using Redis.Common.Dto;
using Redis.Common.Implementations;
using Redis.IntegrationTests.TestEntities;
using Redis.Master;
using Redis.Master.Application;
using Redis.Master.Infrastructure;

namespace Redis.IntegrationTests
{
    [TestClass]
    public class MasterTests
    {
        private HttpClient _masterHttpClient;
        private HttpClient _slaveHttpClient;
        
        private HttpClient _firstFakeChildClient;
        private HttpClient _secondFakeChildClient;
        private HttpClient _thirdFakeChildClient;
        private HttpClient _fourthFakeChildClient;

        private FakeChildClient _firstChildClient;
        private FakeChildClient _secondChildClient;
        private FakeMasterClient _masterClient;

        [TestInitialize]
        public void Init()
        {
            var firstChildServer = CreateChildTestServer();
            var secondChildServer = CreateChildTestServer();
            var thirdChildServer = CreateChildTestServer();
            var fourthChildServer = CreateChildTestServer();

            _firstFakeChildClient = firstChildServer.CreateClient();
            _secondFakeChildClient = secondChildServer.CreateClient();
            _thirdFakeChildClient = thirdChildServer.CreateClient();
            _fourthFakeChildClient = fourthChildServer.CreateClient();

            _firstChildClient = new FakeChildClient();
            _secondChildClient = new FakeChildClient();
            _masterClient = new FakeMasterClient();

            var masterBuilder = new WebHostBuilder()
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("test.appsettings.json", optional: false)
                        .AddEnvironmentVariables();
                })
                .ConfigureTestServices( s =>
                {
                    s.AddSingleton<IMasterClient>(_masterClient);
                    s.AddSingleton<IChildClient>(_firstChildClient);
                    s.AddSingleton<IHashGenerator, JankinsFakeHashGenerator>();
                })
                .UseStartup<Redis.Master.Startup>();

            var slaveBuilder = new WebHostBuilder()
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("slave.appsettings.json", optional: false)
                        .AddEnvironmentVariables();
                })
                .ConfigureTestServices(s =>
                {
                    s.AddSingleton<IChildClient>(_secondChildClient);
                    s.AddSingleton<IHashGenerator, JankinsFakeHashGenerator>();
                })
                .UseStartup<Redis.Master.Startup>();

            var server = new TestServer(masterBuilder);
            var slaveServer = new TestServer(slaveBuilder);

            _masterHttpClient = server.CreateClient();
            _slaveHttpClient = slaveServer.CreateClient();
        }

        [TestMethod]
        public async Task Child_InsertValueViMaster_SeeItInsertedAsync()
        {
            var firstKey = 30;
            var firstVal = "myVal";

            var secondKey = 100;
            var secondVal = "myVal2";

            _firstChildClient.HttpClient = _firstFakeChildClient;
            _secondChildClient.HttpClient = _thirdFakeChildClient;
            _masterClient.HttpClient = _slaveHttpClient;

            await _masterHttpClient.PostAsync($"/master?key={firstKey}",
                new StringContent(JsonConvert.SerializeObject(firstVal), Encoding.UTF8, "application/json"));

            //wait for async replication
            await Task.Delay(3 * 1000);

            _firstChildClient.HttpClient = _secondFakeChildClient;
            _secondChildClient.HttpClient = _fourthFakeChildClient;

            await _masterHttpClient.PostAsync($"/master?key={secondKey}",
                new StringContent(JsonConvert.SerializeObject(secondVal), Encoding.UTF8, "application/json"));

            //wait for async replication
            await Task.Delay(3 * 1000);

            _firstChildClient.HttpClient = _firstFakeChildClient;
            var firstResponse = await _masterHttpClient.GetAsync($"/master?key={firstKey}");
            var firstContent = await firstResponse.Content.ReadAsStringAsync();
           
            _firstChildClient.HttpClient = _secondFakeChildClient;
            var secondResponse = await _masterHttpClient.GetAsync($"/master?key={secondKey}");
            var secondContent = await secondResponse.Content.ReadAsStringAsync();

            _secondChildClient.HttpClient = _thirdFakeChildClient;
            var firstSlaveResponse = await _slaveHttpClient.GetAsync($"/master?key={firstKey}");
            var firstSlaveContent = await firstSlaveResponse.Content.ReadAsStringAsync();
            
            _secondChildClient.HttpClient = _fourthFakeChildClient;
            var secondSlaveResponse = await _masterHttpClient.GetAsync($"/master?key={secondKey}");
            var secondSlaveContent = await secondSlaveResponse.Content.ReadAsStringAsync();

            Assert.AreEqual(firstContent, firstVal);
            Assert.AreEqual(firstContent, firstSlaveContent);
            Assert.AreEqual(secondContent, secondVal);
            Assert.AreEqual(secondContent, secondSlaveContent);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _masterHttpClient.Dispose();
            _slaveHttpClient.Dispose();
            _firstFakeChildClient.Dispose();
            _secondFakeChildClient.Dispose();
            _thirdFakeChildClient.Dispose();
            _fourthFakeChildClient.Dispose();
        }

        private TestServer CreateChildTestServer()
        {
            var hostBuilder = new WebHostBuilder()
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("test.appsettings.json", optional: false)
                        .AddEnvironmentVariables();
                }).UseStartup<Redis.Child.Startup>();

            var server = new TestServer(hostBuilder);
            return server;
        }
    }
}
