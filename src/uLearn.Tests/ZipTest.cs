using System.IO;
using Ionic.Zip;
using NUnit.Framework;

namespace uLearn
{
    [TestFixture]
    public class ZipTest_Test
    {
        [Test]
        public void TestDirRemove()
        {
            var dir = new DirectoryInfo("toZip");
            using (var zip = new ZipFile())
            {
                zip.AddDirectory(dir.FullName);
                zip.RemoveDirectory("toRemove");
                Assert.That(zip.Entries.Count, Is.EqualTo(1));
            }
        }
    }
}
