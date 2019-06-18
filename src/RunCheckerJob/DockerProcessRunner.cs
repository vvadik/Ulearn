using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
		private const string defaultSeccompFilename = "chrome-seccomp.json";

		public static RunningResults Run(ZipRunnerSubmission submission, DockerSandboxRunnerSettings settings, string submissionDirectory)
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
			var isFinished = dockerShellProcess.WaitForExit((int)settings.IdleTimeLimit.TotalMilliseconds);

			if (!isFinished)
			{
				log.Info($"Не хватило времени на работу Docker в папке {dir.FullName}");
				GracefullyShutdownDocker(dockerShellProcess, name, settings);
				return new RunningResults(Verdict.TimeLimit);
			}

			log.Info($"Docker закончил работу и написал: {dockerShellProcess.StandardOutput.ReadToEnd()}");

			if (dockerShellProcess.ExitCode != 0)
			{
				log.Info($"Упал docker в папке {dir.FullName}");
				return new RunningResults(Verdict.SandboxError, error: dockerShellProcess.StandardError.ReadToEnd());
			}

			return LoadResult(dir, settings);
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

		private static RunningResults LoadResult(DirectoryInfo dir, DockerSandboxRunnerSettings settings)
		{
			try
			{
				var resultPath = Path.Combine(dir.GetSubdirectory("output").FullName, "result.json");
				if (new FileInfo(resultPath).Length > settings.OutputLimit)
					return new RunningResults(Verdict.OutputLimit);
				var output = File.ReadAllText(resultPath);
				return new RunningResults(Verdict.Ok, output: output);
			}
			catch (Exception)
			{
				log.Info($"Не удалось прочитать результат тестов в папке {dir.FullName}");
				return new RunningResults(Verdict.SandboxError);
			}
		}

		private static string BuildDockerCommand(DockerSandboxRunnerSettings settings, DirectoryInfo dir, Guid name)
		{
			var seccompPath = ConvertToUnixPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DockerConfig", settings.SeccompFileName ?? defaultSeccompFilename));
			var parts = new List<string>
			{
				"docker run",
				LinkDirectory(dir, "src"),
				LinkDirectory(dir, "tests"),
				LinkDirectory(dir, "output"),
				$"--security-opt seccomp={seccompPath}", //"--privileged",
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

		private static string LinkDirectory(DirectoryInfo rootDirectory, string subdirectory) =>
			$"-v {ConvertToUnixPath(rootDirectory.GetSubdirectory(subdirectory).FullName)}:/app/{subdirectory}";

		private static string ConvertToUnixPath(string path) => path.Replace(@"\", "/");
	}
}