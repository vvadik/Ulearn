namespace RunCheckerJob.Js
{
	public class JsSandboxRunnerSettings: DockerSandboxRunnerSettings
	{
		public JsSandboxRunnerSettings()
			: base("js-sandbox")
		{
			SeccompFileName = "chrome-seccomp.json";
			MemoryLimit = 256 * 1024 * 1024;
		}
	}
}