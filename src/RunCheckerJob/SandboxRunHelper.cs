using System;
using System.IO;
using System.Threading;
using log4net;
using Ulearn.Common;
using Ulearn.Common.Extensions;
using Ulearn.Core.RunCheckerJobApi;

namespace RunCheckerJob
{
	public class SandboxRunHelper
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(SandboxRunHelper));
		public static RunningResults WithSubmissionWorkingDirectory(RunnerSubmission submission, ISandboxRunnerClient runnerClient,
			DirectoryInfo workingDirectory, bool deleteSubmissionsAfterFinish)
		{
			if (!workingDirectory.Exists)
			{
				try
				{
					workingDirectory.Create();
				}
				catch (Exception e)
				{
					log.Error($"Не могу создать директорию для решений: {workingDirectory}", e);
					return new RunningResults(submission.Id, Verdict.SandboxError, error: e.ToString());
				}
			}

			var randomSuffix = Guid.NewGuid().ToString("D");
			randomSuffix = randomSuffix.Substring(randomSuffix.Length - 8);
			var submissionWorkingDirectory = workingDirectory.GetSubdirectory($"{submission.Id}-{randomSuffix}");
			try
			{
				submissionWorkingDirectory.Create();
			}
			catch (Exception e)
			{
				log.Error($"Не могу создать директорию для решения: {submissionWorkingDirectory.FullName}", e);
				return new RunningResults(submission.Id, Verdict.SandboxError, error: e.ToString());
			}

			try
			{
				return runnerClient.RunContainerAndGetResultInternal(submission, submissionWorkingDirectory);
			}
			catch (Exception ex)
			{
				log.Error(ex);
				return new RunningResults(submission.Id, Verdict.SandboxError, error: ex.ToString());
			}
			finally
			{
				if (deleteSubmissionsAfterFinish)
				{
					log.Info($"Удаляю папку с решением: {submissionWorkingDirectory}");
					SafeRemoveDirectory(submissionWorkingDirectory.FullName);
				}
			}
		}
		
		private static void SafeRemoveDirectory(string path)
		{
			try
			{
				/* Sometimes we can't remove directory after Time Limit Exceeded, because process is alive yet. Just wait some seconds before directory removing */
				FuncUtils.TrySeveralTimes(() =>
				{
					Directory.Delete(path, true);
					return true;
				}, 3, () => Thread.Sleep(TimeSpan.FromSeconds(1)));
			}
			catch (Exception e)
			{
				log.Warn($"Произошла ошибка при удалении директории {path}, но я её проигнорирую", e);
			}
		}
	}
}