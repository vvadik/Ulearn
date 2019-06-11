namespace RunCheckerJob
{
	public class CheckerSandboxRunnerSettings : SandboxRunnerSettings
	{
		public CheckerSandboxRunnerSettings(string serviceName)
			: base(serviceName)
		{
			MemoryLimit = 256 * 1024 * 1024;
		}
	}
}