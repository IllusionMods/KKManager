using System;
using KKManager.Data.Zipmods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Data.Zipmods
{
    using CompareTest = Tuple<string, string, bool>;

    [TestClass]
    public class SideloaderVersionComparerTests
    {
        [TestMethod]
        public void SideloaderVersionComparerTest()
        {
            var arr = new[]
            {
                // x, y, true == are equal / false == x < y
                new CompareTest("1.0", "1.1", false),
                new CompareTest("0", "1", false),
                new CompareTest(".0", "0", true),
                new CompareTest(".1", "0.1", true),
                new CompareTest(".2", "2.0", false),
                new CompareTest("r1", "v1.0", true),
                new CompareTest("r1", "v1.0a", false),
                new CompareTest("a", "b", false),
                new CompareTest("a2", "b1", false),
                new CompareTest("a02", "a2", true),
                new CompareTest("", "1", false),
                new CompareTest("", " ", true),
                new CompareTest("..", " ", true),
                new CompareTest("0", " . .", true),
                new CompareTest(null, "1", false),
                new CompareTest(null, " ", true),
            };

            foreach (var test in arr)
            {
                try
                {
                    var equal = test.Item3;
                    if (equal)
                    {
                        // check if results are consistent both ways
                        Assert.AreEqual(0, SideloaderVersionComparer.CompareVersions(test.Item1, test.Item2), $"1 == 2: `{test.Item1}` == `{test.Item2}`");
                        Assert.AreEqual(0, SideloaderVersionComparer.CompareVersions(test.Item2, test.Item1), $"2 == 1: `{test.Item2}` == `{test.Item1}`");
                    }
                    else
                    {
                        Assert.AreEqual(-1, SideloaderVersionComparer.CompareVersions(test.Item1, test.Item2), $"1 < 2: `{test.Item1}` < `{test.Item2}`");
                        Assert.AreEqual(+1, SideloaderVersionComparer.CompareVersions(test.Item2, test.Item1), $"2 > 1: `{test.Item2}` > `{test.Item1}`");
                    }
                }
                catch (AssertFailedException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Assert.Fail(test + " - " + e);
                }
            }
        }
    }
}