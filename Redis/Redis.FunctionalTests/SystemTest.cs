using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Redis.FunctionalTests
{
    //This should be run with docker (docker-compose-tests.yml)
    [TestClass]
    public class SystemTest : IDisposable
    {
        private HttpClient _httpClient;

        private string _firstReplicationUrl;
        private string _secondReplicationUrl;

        [TestInitialize]
        public void Init()
        {
            _firstReplicationUrl = Environment.GetEnvironmentVariable("FIRST_REPLICATION_URL");
            _secondReplicationUrl = Environment.GetEnvironmentVariable("SECOND_REPLICATION_URL");
            Console.WriteLine("url: " + _firstReplicationUrl);
            Console.WriteLine("url2: " + _secondReplicationUrl);
            _httpClient = new HttpClient();
        }

        [TestMethod]
        public async Task System_InsertTwoValues_GetExpected()
        {
            var firstKey = 30;
            var firstVal = "myVal";

            var secondKey = 100;
            var secondVal = "myVal2";

            await _httpClient.PostAsync($"{_firstReplicationUrl}/master?key={firstKey}",
                new StringContent(JsonConvert.SerializeObject(firstVal), Encoding.UTF8, "application/json"));

            await _httpClient.PostAsync($"{_firstReplicationUrl}/master?key={secondKey}",
                new StringContent(JsonConvert.SerializeObject(secondVal), Encoding.UTF8, "application/json"));

            //wait for replication
            await Task.Delay(3000);

            var firstFirstResp = await _httpClient.GetAsync($"{_firstReplicationUrl}/master?key={firstKey}");
            var firstFirstContent = await firstFirstResp.Content.ReadAsStringAsync();

            var firstSecondResp = await _httpClient.GetAsync($"{_secondReplicationUrl}/master?key={firstKey}");
            var firstSecondContent = await firstSecondResp.Content.ReadAsStringAsync();

            var secondFirstResp = await _httpClient.GetAsync($"{_firstReplicationUrl}/master?key={secondKey}");
            var secondFirstContent = await secondFirstResp.Content.ReadAsStringAsync();

            var secondSecondResp = await _httpClient.GetAsync($"{_secondReplicationUrl}/master?key={secondKey}");
            var secondSecondContent = await secondSecondResp.Content.ReadAsStringAsync();

            Assert.AreEqual(firstFirstContent, firstVal);
            Assert.AreEqual(firstFirstContent, firstSecondContent);
            Assert.AreEqual(secondFirstContent, secondVal);
            Assert.AreEqual(secondFirstContent, secondSecondContent);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SystemTest()
        {
            Dispose(false);
        }
    }
}
