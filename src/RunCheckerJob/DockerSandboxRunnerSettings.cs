using System;
using JetBrains.Annotations;

namespace RunCheckerJob
{
	public class DockerSandboxRunnerSettings : SandboxRunnerSettings
	{
		[NotNull]public string SandBoxName;
		[CanBeNull]public string SeccompFileName;
		[NotNull]public string RunCommand;
		
		public DockerSandboxRunnerSettings(string sandBoxName, string runCommand)
		{
			SandBoxName = sandBoxName;
			RunCommand = runCommand;
			MemoryLimit = 256 * 1024 * 1024;
			TestingTimeLimit = TimeSpan.FromSeconds(10);
			MaintenanceTimeLimit = TimeSpan.FromSeconds(10);
		}
	}
}