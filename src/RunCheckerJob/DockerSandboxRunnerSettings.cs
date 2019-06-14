using JetBrains.Annotations;

namespace RunCheckerJob
{
	public class DockerSandboxRunnerSettings : SandboxRunnerSettings
	{
		[NotNull]public string SandBoxName;
		[CanBeNull]public string SeccompFileName;
		
		protected DockerSandboxRunnerSettings(string sandBoxName)
		{
			SandBoxName = sandBoxName;
			MemoryLimit = 256 * 1024 * 1024;
		}
	}
}