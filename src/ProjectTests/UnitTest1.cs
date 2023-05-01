using NUnit.Framework;
using KKManager;
using KKManager.Data.Game;

namespace ProjectTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
    [TestFixture]
    public class GameNameTests
    {
        [Test]
        public void TestLongNameToShortName()
        {
            var gameName = new GameName("HoneySelect2");
            Assert.AreEqual("HS2", gameName.ShortName);
        }

        [Test]
        public void TestShortNameToLongName()
        {
            var gameName = new GameName("HS2");
            Assert.AreEqual("HoneySelect2", gameName.LongName);
        }

        [Test]
        public void TestEquals()
        {
            var gameName1 = new GameName("HoneySelect2");
            var gameName2 = new GameName("HS2");
            Assert.AreEqual(gameName1, gameName2);
            Assert.AreEqual(new GameName("HS2"), "HoneySelect2");
            Assert.AreEqual(gameName2, "HS2");
        }

        [Test]
        public void TestInvalidName()
        {
            var gameName = new GameName("InvalidName");
            Assert.IsNull(gameName.LongName);
            Assert.IsNull(gameName.ShortName);
        }

        [Test]
        public void TestCase()
        {
            var gameName1 = new GameName("hs2");
            var gameName2 = new GameName("HS2");
            Assert.AreEqual(gameName1, gameName2);
        }
    }
}
