using System;
using System.IO;
using RunCheckerJob.Js;
using Ulearn.Core;
using Ulearn.Core.RunCheckerJobApi;

namespace RunCheckerJob
{
	internal class DockerSandboxRunner : ISandboxRunnerClient
	{
		public RunningResults Run(RunnerSubmission submission)
		{
			return SandboxRunHelper.WithSubmissionWorkingDirectory(submission, this, SandboxRunnerSettings.WorkingDirectory, SandboxRunnerSettings.DeleteSubmissionsAfterFinish);
		}

		private (IResultParser, DockerSandboxRunnerSettings) GetLanguageSpecific(RunnerSubmission submission)
		{
			switch (submission)
			{
				case ZipRunnerSubmission s when s.Language == Language.JavaScript:
					return (new JsResultParser(), new JsSandboxRunnerSettings());
				default:
					throw new NotSupportedException($"Submission {submission} is not supported");
			}
		} 

		public RunningResults RunContainerAndGetResultInternal(RunnerSubmission submission, DirectoryInfo submissionWorkingDirectory)
		{
			var (resultParser, settings) = GetLanguageSpecific(submission);
			var result = DockerProcessRunner.Run((ZipRunnerSubmission)submission, settings, submissionWorkingDirectory.FullName);
			if (result.Verdict == Verdict.Ok)
				result = resultParser.Parse(result.Output);
			result.Id = submission.Id;
			return result;
		}
	}
}