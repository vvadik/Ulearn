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
		public DirectoryInfo WorkingDirectory;
		public bool DeleteSubmissionsAfterFinish;
		public string ServiceName;

		protected SandboxRunnerSettings(string serviceName)
		{
			ServiceName = serviceName;
			var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			WorkingDirectory = new DirectoryInfo(Path.Combine(baseDirectory, "submissions"));
			var deleteSubmissions = bool.Parse(ConfigurationManager.AppSettings[$"ulearn.{ServiceName}.deleteSubmissions"] ?? "true");
			DeleteSubmissionsAfterFinish = deleteSubmissions;
			var workingDirectory = ConfigurationManager.AppSettings[$"ulearn.{ServiceName}.submissionsWorkingDirectory"];
			if (!string.IsNullOrWhiteSpace(workingDirectory))
				WorkingDirectory = new DirectoryInfo(workingDirectory);
		}
	}
}