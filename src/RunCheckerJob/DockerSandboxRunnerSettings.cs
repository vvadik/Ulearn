using JetBrains.Annotations;

namespace RunCheckerJob
{
	public class DockerSandboxRunnerSettings : SandboxRunnerSettings
	{
		[NotNull]public string SandBoxName;
		[CanBeNull]public string SeccompFileName;
		
		public DockerSandboxRunnerSettings(string serviceName, string sandBoxName)
			: base(serviceName)
		{
			SandBoxName = sandBoxName;
			MemoryLimit = 256 * 1024 * 1024;
		}
	}
}