using System.IO;

namespace RunCheckerJob.Api
{
	public interface ISandboxRunnerClient
	{
		RunningResults Run(RunnerSubmission submission);
	}
}