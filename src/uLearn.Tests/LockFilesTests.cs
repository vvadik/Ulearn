using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace uLearn
{
	[TestFixture]
	class LockFilesTests
	{
		[Test]
		public void CreateLockFile()
		{
			var current = new DirectoryInfo(".");
			var tempFileName = CreateTempFile();
			var lockFileName = current.GetFile("lock-file").FullName;
			File.WriteAllText(lockFileName, "test");
			try
			{
				new FileInfo(tempFileName).MoveTo(lockFileName);
				throw new AssertionException("MoveTo() should create IOException if destination file already exists");
			}
			catch (IOException)
			{
			}
			File.ReadAllText(lockFileName).ShouldBeEquivalentTo("temp");
		}

		private string CreateTempFile()
		{
			var tempFileName = Path.GetTempFileName();
			File.WriteAllText(tempFileName, "temp");
			return tempFileName;
		}
	}
}
