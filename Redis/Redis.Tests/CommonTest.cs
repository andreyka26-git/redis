using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Redis.Common.Abstractions;
using Redis.Common.Implementations;
using Redis.Tests.TestEntities;

namespace Redis.Tests
{
    [TestClass]
    public class CommonTest
    {
        //it's not really unit test, because we're going validity of external library, but we want to sleep well at night, right?
        [TestMethod]
        public void JenkinsHash_CreateHash_Success()
        {
            var testData = "simpleString";
            var serializerMock = new Mock<IBinarySerializer>();
            serializerMock.Setup(s => s.Serialize(It.IsAny<object>())).Returns(() => Encoding.UTF8.GetBytes(testData));

            var bitHelperFake = new BitHelperFake();

            var hashGenerator = new JenkinsHashGenerator(serializerMock.Object, bitHelperFake);

            var firstHash = hashGenerator.GenerateHash(testData);
            var secondHash = hashGenerator.GenerateHash(testData);
            Assert.AreEqual(firstHash, secondHash);
        }

        [TestMethod]
        public void BitHelper_ConvertToInt_Success()
        {
            var helper = new BitHelper();
            var helperFake = new BitHelperFake();

            var val = int.MaxValue - 1234567;
            var valBytes = BitConverter.GetBytes(val);

            var first = helper.BytesToUInt32(valBytes);
            var second = helper.BytesToUInt32(valBytes);

            Assert.AreEqual(first, second);
            Assert.AreEqual(first, (uint)val);
        }

        [TestMethod]
        public void BitHelper_ConvertToInt_ThrowIfBytesCountMoreThan4()
        {
            var helper = new BitHelper();

            var val = Int64.MaxValue - 1234567;
            var valBytes = BitConverter.GetBytes(val);

            Assert.ThrowsException<Exception>(() => helper.BytesToUInt32(valBytes));
        }
    }
}
