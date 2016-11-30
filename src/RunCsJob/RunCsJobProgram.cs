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

			var pathToCompiler = args.Any(x => x.StartsWith("-p:"))
				? args.FirstOrDefault(x => x.StartsWith("-p:"))?.Substring(3)
				: Path.Combine(new DirectoryInfo(".").FullName, "Microsoft.Net.Compilers.1.3.2");

			if (args.Contains("--selfcheck"))
				SelfCheck(pathToCompiler);
			else
				new RunCsJobProgram().Run(pathToCompiler);
		}

		public void Run(string pathToCompiler)
		{
			AppDomain.MonitoringIsEnabled = true;
			log.Info($"Monitoring {address} for new submissions");

			Client client;
			try
			{
				client = new Client(address, token);
			}
			catch (Exception e)
			{
				log.Error("Can't create Client for monitoring ulearn", e);
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
					log.Error($"Can't get new submissions from ulearn. Try again after {sleep.TotalSeconds} seconds", e);
					Thread.Sleep(sleep);
					continue;
				}

				log.Info($"Received {newUnhandled.Count} submissions: [{string.Join(", ", newUnhandled.Select(s => s.Id))}]");

				if (newUnhandled.Any())
				{
					var results = newUnhandled.Select(unhandled => SandboxRunner.Run(pathToCompiler, unhandled)).ToList();
					log.Info($"Results: [{string.Join(", ", results.Select(r => r.Verdict))}]");
					try
					{
						client.SendResults(results);
					}
					catch (Exception e)
					{
						log.Error("Can't send run results back to ulearn", e);
					}
				}
				Thread.Sleep(sleep);
			}
		}

		private static void SelfCheck(string pathToCompiler)
		{
			var res = SandboxRunner.Run(pathToCompiler,
				new FileRunnerSubmission
				{
					Id = Utils.NewNormalizedGuid(),
					NeedRun = true,
					Code = "class C { static void Main(){ System.Console.WriteLine(\"Привет мир!\");}}"
				});
			Console.WriteLine(res);
		}
	}
}