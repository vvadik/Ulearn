using System;
using System.IO;
using System.Threading;
using log4net;
using Metrics;
using Ulearn.Common;
using Ulearn.Common.Extensions;
using Ulearn.Core;
using Ulearn.Core.RunCheckerJobApi;

namespace RunCheckerJob
{
	public class CheckerSandboxRunnerClient : ISandboxRunnerClient
	{
		private readonly CheckerSandboxRunnerSettings settings;
		
		public CheckerSandboxRunnerClient(CheckerSandboxRunnerSettings settings)
		{
			this.settings = settings;
		}
		
		public RunningResults Run(RunnerSubmission submission)
		{
			return SandboxRunner.Run(submission, settings.ServiceName, settings);
		}
	}
	
	public class SandboxRunner
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(SandboxRunner));
		//todo send metrics about docker
		private readonly MetricSender metricSender;

		private readonly RunnerSubmission submission;
		private readonly SandboxRunnerSettings settings;

		// todo сделать с этим что-то
		private bool hasTimeLimit;
		private bool hasMemoryLimit;
		private bool hasOutputLimit;

		public static RunningResults Run(RunnerSubmission submission, string serviceName, CheckerSandboxRunnerSettings settings = null)
		{
			settings = settings ?? new CheckerSandboxRunnerSettings(serviceName);
			var workingDirectory = settings.WorkingDirectory;
			if (!workingDirectory.Exists)
			{
				try
				{
					workingDirectory.Create();
				}
				catch (Exception e)
				{
					log.Error($"Не могу создать директорию для компиляции решений: {workingDirectory}", e);
					return new RunningResults(submission.Id, Verdict.SandboxError, error: e.ToString());
				}
			}

			var randomSuffix = Guid.NewGuid().ToString("D");
			randomSuffix = randomSuffix.Substring(randomSuffix.Length - 8);
			var submissionCompilationDirectory = workingDirectory.GetSubdirectory($"{submission.Id}-{randomSuffix}");
			try
			{
				submissionCompilationDirectory.Create();
			}
			catch (Exception e)
			{
				log.Error($"Не могу создать директорию для компиляции решения: {submissionCompilationDirectory.FullName}", e);
				return new RunningResults(submission.Id, Verdict.SandboxError, error: e.ToString());
			}

			try
			{
				RunningResults result;
				var instance = new SandboxRunner(submission, serviceName, settings);
				switch (submission)
				{
					case ZipRunnerSubmission _:
						result = instance.RunJsBuild(submissionCompilationDirectory.FullName);
						break;
					default:
						throw new NotSupportedException($"Submission {submission} is not supported");
				}

				result.Id = submission.Id;
				return result;
			}
			catch (Exception ex)
			{
				log.Error(ex);
				return new RunningResults(submission.Id, Verdict.SandboxError, error: ex.ToString());
			}
			finally
			{
				if (settings.DeleteSubmissionsAfterFinish)
				{
					log.Info($"Удаляю папку с решением: {submissionCompilationDirectory}");
					SafeRemoveDirectory(submissionCompilationDirectory.FullName);
				}
			}
		}

		public SandboxRunner(RunnerSubmission submission, string serviceName, CheckerSandboxRunnerSettings settings = null)
		{
			this.submission = submission;
			metricSender = new MetricSender(serviceName);
			this.settings = settings ?? new CheckerSandboxRunnerSettings(serviceName);
		}

		private RunningResults RunJsBuild(string submissionCompilationDirectory)
		{
			var jsSubmission = (ZipRunnerSubmission)submission;
			log.Info($"Запускаю проверку Js-решения {jsSubmission.Id}");
			var dir = new DirectoryInfo(submissionCompilationDirectory);

			try
			{
				Utils.UnpackZip(jsSubmission.ZipFileData, dir.FullName);
			}
			catch (Exception ex)
			{
				log.Error("Не могу распаковать решение", ex);
				return new RunningResults(jsSubmission.Id, Verdict.SandboxError, error: ex.ToString());
			}

			log.Info($"Запускаю Docker для решения {jsSubmission.Id} в папке {dir.FullName}");

			return JsRunner.Run(settings, dir);
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