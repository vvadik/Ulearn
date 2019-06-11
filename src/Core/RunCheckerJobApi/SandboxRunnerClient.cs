namespace Ulearn.Core.RunCheckerJobApi
{
	public interface ISandboxRunnerClient
	{
		RunningResults Run(RunnerSubmission submission);
	}
}