using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Redis.Common.Abstractions;
using Redis.Master;
using Redis.Master.Application;
using Redis.Master.Infrastructure;
using Redis.Tests.TestEntities;

namespace Redis.Tests
{
    [TestClass]
    public class MasterTest
    {
        private MasterService _master;

        [TestInitialize]
        public void Init()
        {
            IHashGenerator hashGeneratorFake = new HashGeneratorFake();
            var clientMock = new Mock<IChildClient>();
            clientMock
                .Setup(c => c.AddAsync(
                    It.IsAny<Master.Application.Child>(),
                    It.IsAny<string>(),
                    It.IsAny<uint>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()));

            clientMock
                .Setup(c => c.GetAsync(
                    It.IsAny<Master.Application.Child>(),
                    It.IsAny<string>(),
                    It.IsAny<uint>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => "someValue");

            var optionsMock = new Mock<IOptions<MasterOptions>>();

            optionsMock.Setup(o => o.Value).Returns(() => new MasterOptions
            {
                Children = new List<string> {"url1", "url2", "url3"},
                PartitionItemsCount = 30
            });

            var primeNumberService = new PrimeNumberServiceFake();

            _master = new MasterService(hashGeneratorFake, clientMock.Object, primeNumberService, optionsMock.Object);
        }

        [TestMethod]
        public void Master_DetermineChild_Success()
        {
            var children = new List<Master.Application.Child>
            {
                new Master.Application.Child("url1", 0, 29),
                new Master.Application.Child("url2", 30, 59),
                new Master.Application.Child("url3", 60, 89),
            };

            var overallCount = 90;
            var firstChild = _master.DetermineChildByHash(children, "key", 3, overallCount);
            var secondChild = _master.DetermineChildByHash(children, "key", 30, overallCount);
            var thirdChild = _master.DetermineChildByHash(children, "key", 89, overallCount);

            Assert.AreEqual(firstChild.ChildUrl, children[0].ChildUrl);
            Assert.AreEqual(secondChild.ChildUrl, children[1].ChildUrl);
            Assert.AreEqual(thirdChild.ChildUrl, children[2].ChildUrl);
        }

        [TestMethod]
        public void Master_Initialize_Success()
        {
            var children = _master.InitializeChildren(new List<string> {"url1", "url2", "url3"}, 30);

            Assert.AreEqual(children[0].MinHash, 0);
            Assert.AreEqual(children[0].MaxHash, 29);
            Assert.AreEqual(children[1].MinHash, 30);
            Assert.AreEqual(children[1].MaxHash, 59); 
            Assert.AreEqual(children[2].MinHash, 60);
            Assert.AreEqual(children[2].MaxHash, 89);
        }
    }
}
