using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using log4net;
using NUnit.Framework;
using Ulearn.Core;
using Ulearn.Core.RunCheckerJobApi;

namespace RunCheckerJob
{
	internal class SelfChecker
	{
		private readonly DockerSandboxRunner sandboxRunner;
		private static readonly string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
		private static readonly ILog log = LogManager.GetLogger(typeof(SelfChecker));

		public SelfChecker(DockerSandboxRunner sandboxRunner)
		{
			this.sandboxRunner = sandboxRunner;
		}
		
		public RunningResults JsSelfCheck()
		{
			var directory = Path.Combine(baseDirectory, "../../../../../sandbox/js/sample");
			var bytes = ZipHelper.CreateFromDirectory(directory, CompressionLevel.Fastest, false, Encoding.UTF8, null).ToArray();
			var res = sandboxRunner.Run(new CommandRunnerSubmission
			{
				Id = Utils.NewNormalizedGuid(),
				Language = Language.JavaScript,
				ZipFileData = bytes
			});
			log.Info(res);
			return res;
		}
	}

	[TestFixture, Explicit]
	internal class SelfCheckerTests
	{
		[Test, Explicit]
		public void JsSelfCheckTest()
		{
			var res = new SelfChecker(new DockerSandboxRunner())
				.JsSelfCheck();
			Assert.AreEqual(Verdict.RuntimeError, res.Verdict);
			Assert.AreEqual("sum might fail: expected true to equal false", res.Error);
		}
	}
}