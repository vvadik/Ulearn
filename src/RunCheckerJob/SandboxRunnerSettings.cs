using System;
using System.Configuration;
using System.IO;

namespace RunCheckerJob
{
	public class SandboxRunnerSettings
	{
		public TimeSpan CompilationTimeLimit = TimeSpan.FromSeconds(10);
		private const int timeLimitInSeconds = 10;
		public TimeSpan TimeLimit = TimeSpan.FromSeconds(timeLimitInSeconds);
		public TimeSpan IdleTimeLimit = TimeSpan.FromSeconds(2 * timeLimitInSeconds);
		public int MemoryLimit = 64 * 1024 * 1024;
		public int OutputLimit = 10 * 1024 * 1024;
		public TimeSpan WaitSandboxAfterKilling = TimeSpan.FromSeconds(5);
		
		public static readonly DirectoryInfo WorkingDirectory;
		public static readonly bool DeleteSubmissionsAfterFinish;

		static SandboxRunnerSettings()
		{
			var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			WorkingDirectory = new DirectoryInfo(Path.Combine(baseDirectory, "submissions"));
			var deleteSubmissions = bool.Parse(ConfigurationManager.AppSettings["ulearn.checker.deleteSubmissions"] ?? "true");
			DeleteSubmissionsAfterFinish = deleteSubmissions;
			var workingDirectory = ConfigurationManager.AppSettings["ulearn.checker.submissionsWorkingDirectory"];
			if (!string.IsNullOrWhiteSpace(workingDirectory))
				WorkingDirectory = new DirectoryInfo(workingDirectory);
		}
	}
}