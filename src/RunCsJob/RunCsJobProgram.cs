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
		private const string CompilersFolderName = "Microsoft.Net.Compilers.1.3.2";

		private readonly string address;
		private readonly string token;
		private readonly TimeSpan sleep;
		private readonly int jobsToRequest;
		private static string pathToCompiler;

		private readonly ManualResetEvent shutdownEvent = new ManualResetEvent(false);
		private static readonly ILog log = LogManager.GetLogger(typeof(RunCsJobProgram));

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

				var baseDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
				if (string.IsNullOrEmpty(baseDirectory))
				{
					log.Error($"Не могу определить текущую директорию. System.Reflection.Assembly.GetExecutingAssembly().CodeBase вернул {System.Reflection.Assembly.GetExecutingAssembly().CodeBase}");
					Environment.Exit(1);
				}
				pathToCompiler = Path.Combine(baseDirectory, CompilersFolderName);
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

			if (args.Any(x => x.StartsWith("-p:")))
				pathToCompiler = args.FirstOrDefault(x => x.StartsWith("-p:"))?.Substring(3);

			if (args.Contains("--selfcheck"))
				SelfCheck(pathToCompiler);
			else
				new RunCsJobProgram().Run();
		}

		public void Run()
		{
			if (!Directory.Exists(pathToCompiler))
			{
				log.Error($"Не найдена папка с компиляторами: {pathToCompiler}");
				Environment.Exit(1);
			}
			log.Info($"Путь до компиляторов: {pathToCompiler}");

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
			MainLoop(pathToCompiler, client);
		}

		private void MainLoop(string pathToCompiler, Client client)
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
					var results = newUnhandled.Select(unhandled => SandboxRunner.Run(unhandled, pathToCompiler)).ToList();
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

		private static void SelfCheck(string pathToCompiler)
		{
			var res = SandboxRunner.Run(new FileRunnerSubmission
			{
				Id = Utils.NewNormalizedGuid(),
				NeedRun = true,
				Code = "class C { static void Main(){ System.Console.WriteLine(\"Привет мир!\");}}"
			}, pathToCompiler);
			log.Info(res);
		}
	}
}