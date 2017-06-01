using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using log4net;
using log4net.Config;
using RunCsJob.Api;
using uLearn;

namespace RunCsJob
{
	public class RunCsJobProgram
	{
		private readonly string address;
		private readonly string token;
		private readonly TimeSpan sleep;
		private readonly int jobsToRequest;

		private readonly ManualResetEvent shutdownEvent = new ManualResetEvent(false);
		private static readonly ILog log = LogManager.GetLogger(typeof(RunCsJobProgram));
		public readonly SandboxRunnerSettings Settings;

		public RunCsJobProgram(ManualResetEvent externalShutdownEvent=null)
		{
			if (externalShutdownEvent != null)
				shutdownEvent = externalShutdownEvent;

			try
			{
				address = ConfigurationManager.AppSettings["submissionsUrl"];
				token = ConfigurationManager.AppSettings["runnerToken"];
				sleep = TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings["sleepSeconds"] ?? "1"));
				jobsToRequest = int.Parse(ConfigurationManager.AppSettings["jobsToRequest"] ?? "5");
				var deleteSubmissions = bool.Parse(ConfigurationManager.AppSettings["ulearn.runcsjob.deleteSubmissions"] ?? "true");
				Settings = new SandboxRunnerSettings
				{
					DeleteSubmissionsAfterFinish = deleteSubmissions,
				};
				var workingDirectory = ConfigurationManager.AppSettings["ulearn.runcsjob.submissionsWorkingDirectory"];
				if (!string.IsNullOrWhiteSpace(workingDirectory))
					Settings.WorkingDirectory = new DirectoryInfo(workingDirectory);
			}
			catch (Exception e)
			{
				log.Error(e);
				throw;
			}
		}

		public static void Main(string[] args)
		{
			XmlConfigurator.Configure();

			var program = new RunCsJobProgram();
			if (args.Any(x => x.StartsWith("-p:")))
			{
				var path = args.FirstOrDefault(x => x.StartsWith("-p:"))?.Substring(3);
				if (path != null)
					program.Settings.MsBuildSettings.CompilerDirectory = new DirectoryInfo(path);
			}
			if (args.Contains("--selfcheck"))
				program.SelfCheck();
			else
				program.Run();
		}

		public void Run()
		{
			if (!Settings.MsBuildSettings.CompilerDirectory.Exists)
			{
				log.Error($"Не найдена папка с компиляторами: {Settings.MsBuildSettings.CompilerDirectory}");
				Environment.Exit(1);
			}
			log.Info($"Путь до компиляторов: {Settings.MsBuildSettings.CompilerDirectory}");

			AppDomain.MonitoringIsEnabled = true;
			log.Info($"Отправляю запросы на {address} для получения новых решений");

			Client client;
			try
			{
				client = new Client(address, token);
			}
			catch (Exception e)
			{
				log.Error("Не могу создать HTTP-клиента для отправки запроса на ulearn", e);
				throw;
			}
			MainLoop(client);
		}

		private void MainLoop(Client client)
		{
			while (!shutdownEvent.WaitOne(0))
			{
				List<RunnerSubmission> newUnhandled;
				try
				{
					newUnhandled = client.TryGetSubmissions(jobsToRequest).Result;
				}
				catch (Exception e)
				{
					log.Error($"Не могу получить решения из ulearn. Следующая попытка через {sleep.TotalSeconds} секунд", e);
					Thread.Sleep(sleep);
					continue;
				}

				log.Info($"Получил {newUnhandled.Count} решение(й) со следующими ID: [{string.Join(", ", newUnhandled.Select(s => s.Id))}]");

				if (newUnhandled.Any())
				{
					var results = newUnhandled.Select(unhandled => SandboxRunner.Run(unhandled, Settings)).ToList();
					log.Info($"Результаты проверки: [{string.Join(", ", results.Select(r => r.Verdict))}]");
					try
					{
						client.SendResults(results);
					}
					catch (Exception e)
					{
						log.Error("Не могу отправить результаты проверки на ulearn", e);
					}
				}
				Thread.Sleep(sleep);
			}
		}

		private void SelfCheck()
		{
			var res = SandboxRunner.Run(new FileRunnerSubmission
			{
				Id = Utils.NewNormalizedGuid(),
				NeedRun = true,
				Code = "class C { static void Main(){ System.Console.WriteLine(\"Привет мир!\");}}"
			}, Settings);
			log.Info(res);
		}
	}
}
 