using NUnit.Framework;

namespace Core.Specs
{
    [TestFixture]
    public class Specification
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
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
