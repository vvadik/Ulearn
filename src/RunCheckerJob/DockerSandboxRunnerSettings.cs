using System;
using JetBrains.Annotations;

namespace RunCheckerJob
{
	public class DockerSandboxRunnerSettings : SandboxRunnerSettings
	{
		[NotNull] public string SandBoxName;
		[CanBeNull] public string SeccompFileName;
		[NotNull] public string RunCommand;
		public int MemorySwapLimit; // The amount of combined memory and swap that can be used

		public DockerSandboxRunnerSettings(string sandBoxName, string runCommand)
		{
			SandBoxName = sandBoxName;
			RunCommand = runCommand;
			MemoryLimit = 256 * 1024 * 1024;
			MemorySwapLimit = MemoryLimit; // If MemoryLimit and MemorySwapLimit are set to the same value, this prevents containers from using any swap.
			if(sandBoxName == "js-sandbox")
				TestingTimeLimit = TimeSpan.FromSeconds(100);
			else
				TestingTimeLimit = TimeSpan.FromSeconds(10);
			MaintenanceTimeLimit = TimeSpan.FromSeconds(10);
		}
	}
}