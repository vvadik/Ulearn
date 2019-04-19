using System.IO;
using System.Reflection;
using NUnit.Framework;
using Serilog;

namespace GitCourseUpdater
{
	[TestFixture, Explicit]
	public class GitRepo_Tests
	{
		[Test, Explicit]
		public void Test()
		{
			var log = new LoggerConfiguration().CreateLogger();
			var url = "git@github.com:vorkulsky/git_test.git";
			var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var reposBaseDir = Path.Combine(assemblyPath, "repos");
			var privateKeyPassphrase = "test";
			var privateKeyPath = new FileInfo(Path.Combine(assemblyPath, "../../../TestResources/test_key"));
			var publicKeyPath = new FileInfo(Path.Combine(assemblyPath, "../../../TestResources/test_pub_key.pub"));
			using (IGitRepo repo = new GitRepo(url, reposBaseDir, publicKeyPath, privateKeyPath, privateKeyPassphrase, log))
			{
				var zip = repo.GetMasterLastCommitStateAsZip();
				File.WriteAllBytes(Path.Combine(assemblyPath, "result.zip"), zip.ToArray());
				var lastCommitInfo = repo.GetMasterLastCommitInfo();
				var info = repo.GetCommitInfo("37d6b0857fd3ae6135dcd2cec6899c1e318f9040");
				var files = repo.GetChangedFiles("37d6b0857fd3ae6135dcd2cec6899c1e318f9040", "bd2f024b57ea7b603447c5dd6e650d0e72a8c6d0");
			}
			Assert.Pass();
		}
	}
}