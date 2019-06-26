using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Ulearn.Common.Extensions;
using Ulearn.Core;
using Ulearn.Core.RunCheckerJobApi;

namespace RunCheckerJob
{
	internal class DockerProcessRunner
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(DockerProcessRunner));

		static DockerProcessRunner()
		{
			/* We should register encoding provider for Encoding.GetEncoding(1251) works */
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		}

		public static RunningResults Run(CommandRunnerSubmission submission, DockerSandboxRunnerSettings settings, string submissionDirectory)
		{
			log.Info($"Запускаю проверку решения {submission.Id}");
			var dir = new DirectoryInfo(submissionDirectory);

			try
			{
				Utils.UnpackZip(submission.ZipFileData, dir.FullName);
			}
			catch (Exception ex)
			{
				log.Error("Не могу распаковать решение", ex);
				return new RunningResults(submission.Id, Verdict.SandboxError, error: ex.ToString());
			}
			
			var outputDirectory = dir.GetSubdirectory("output");
			try
			{
				outputDirectory.Create();
			}
			catch (Exception e)
			{
				log.Error($"Не могу создать директорию для результатов решения: {dir.FullName}", e);
				return new RunningResults(Verdict.SandboxError, error: e.ToString());
			}

			log.Info($"Запускаю Docker для решения {submission.Id} в папке {dir.FullName}");

			return RunDocker(settings, dir);
		}
		
		private static RunningResults RunDocker(DockerSandboxRunnerSettings settings, DirectoryInfo dir)
		{
			var name = Guid.NewGuid();
			var dockerCommand = BuildDockerCommand(settings, dir, name);
			var dockerShellProcess = BuildShellProcess(dockerCommand);

			dockerShellProcess.Start();
			var readErrTask = new AsyncReader(dockerShellProcess.StandardError, settings.OutputLimit).GetDataAsync();
			var readOutTask = new AsyncReader(dockerShellProcess.StandardOutput, settings.OutputLimit).GetDataAsync();
			var isFinished = Task.WaitAll(new Task[] { readErrTask, readOutTask }, (int)(settings.MaintenanceTimeLimit + settings.TestingTimeLimit).TotalMilliseconds);

			if (readErrTask.Result.Length > settings.OutputLimit || readOutTask.Result.Length > settings.OutputLimit)
			{
				log.Warn("Программа вывела слишком много");
				if(!dockerShellProcess.HasExited)
					GracefullyShutdownDocker(dockerShellProcess, name, settings);
				return new RunningResults(Verdict.OutputLimit);
			}
			
			if (!isFinished || !dockerShellProcess.HasExited)
			{
				log.Info($"Не хватило времени на работу Docker в папке {dir.FullName}");
				GracefullyShutdownDocker(dockerShellProcess, name, settings);
				return new RunningResults(Verdict.TimeLimit);
			}

			log.Info($"Docker закончил работу и написал: {readOutTask.Result}");

			if (dockerShellProcess.ExitCode != 0)
			{
				log.Info($"Упал в папке {dir.FullName}");
				log.Warn($"Docker написал в stderr:\n{readErrTask.Result}");
				return new RunningResults(Verdict.SandboxError, error: readErrTask.Result);
			}

			return new RunningResults(Verdict.Ok, output: readOutTask.Result, error: readErrTask.Result);
		}

		private static void GracefullyShutdownDocker(Process dockerShellProcess, Guid name, SandboxRunnerSettings settings)
		{
			try
			{
				dockerShellProcess.Kill();
			}
			catch (Win32Exception)
			{
				/* Sometimes we can catch Access Denied error because the process is already terminating. It's ok, we don't need to rethrow exception */
			}
			catch (InvalidOperationException)
			{
				/* If process has already terminated */
			}

			Task.Run(() =>
			{
				var cleanup1 = BuildShellProcess($"docker container rm -f {name}");
				cleanup1.Start();
				var isCleanupFinished1 = cleanup1.WaitForExit((int)settings.WaitSandboxAfterKilling.TotalMilliseconds);
				if (isCleanupFinished1)
					log.Info($"Повисший контейнер {name} очищен");
				else
					log.Error($"Не удалось очистить повисший контейнер {name}");
			});
		}

		private static Process BuildShellProcess(string shellCommand)
		{
			return new Process
			{
				StartInfo =
				{
					Arguments = $"/C {shellCommand}",
					FileName = "cmd.exe",
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true,
					UseShellExecute = false
				}
			};
		}

		private static string BuildDockerCommand(DockerSandboxRunnerSettings settings, DirectoryInfo dir, Guid name)
		{
			var seccompPath = settings.SeccompFileName == null ? null : ConvertToUnixPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DockerConfig", settings.SeccompFileName));
			var parts = new List<string>
			{
				"docker run",
				$"-v {ConvertToUnixPath(dir.FullName)}:/app/src",
				seccompPath == null ? "" : $"--security-opt seccomp={seccompPath}", //"--privileged",
				"--network none",
				"--restart no",
				"--rm",
				$"--name {name}",
				//"-it",
				$"-m {settings.MemoryLimit}b",
				settings.SandBoxName,
			};
			return string.Join(" ", parts);
		}

		private static string ConvertToUnixPath(string path) => path.Replace(@"\", "/");
	}
}