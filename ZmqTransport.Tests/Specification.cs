using Core;
using Moq;
using NUnit.Framework;

namespace ZmqTransport.Tests
{
    [TestFixture]
    public class Specification
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            Logger.Set(new Mock<ILog>().Object);

            Given();
            When();
        }

        public virtual void Given()
        {
        }

        public virtual void When()
        {
        }
    }
}
