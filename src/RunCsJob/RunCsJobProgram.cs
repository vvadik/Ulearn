using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using RunCsJob.Api;
using uLearn;

namespace RunCsJob
{
	internal class RunCsJobProgram
	{
		private readonly string address;
		private readonly string token;
		private readonly TimeSpan sleep;
		private readonly int jobsToRequest;

		private RunCsJobProgram()
		{
			address = ConfigurationManager.AppSettings["submissionsUrl"];
			token = ConfigurationManager.AppSettings["runnerToken"];
			sleep = TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings["sleepSeconds"] ?? "1"));
			jobsToRequest = int.Parse(ConfigurationManager.AppSettings["jobsToRequest"] ?? "5");
		}

		public static void Main(string[] args)
		{
			var pathToCompiler = args.Any(x => x.StartsWith("-p:"))
				? args.FirstOrDefault(x => x.StartsWith("-p:"))?.Substring(3)
				: Path.Combine(new DirectoryInfo(".").FullName, "Microsoft.Net.Compilers.1.3.2", "tools");

			if (args.Contains("--selfcheck"))
				SelfCheck(pathToCompiler);
			else
				new RunCsJobProgram().Run(pathToCompiler);
		}

		private void Run(string pathToCompiler)
		{
			AppDomain.MonitoringIsEnabled = true;
			Console.WriteLine($"Listen {address}");
			var client = new Client(address, token);
			MainLoop(pathToCompiler, client);
		}

		private void MainLoop(string pathToCompiler, Client client)
		{
			while (true)
			{
				var newUnhandled = client.TryGetSubmissions(jobsToRequest).Result;
				Console.WriteLine($"Received {newUnhandled.Count} submissions: [{string.Join(", ", newUnhandled.Select(s => s.Id))}]");

				if (newUnhandled.Any())
				{
					var results = newUnhandled.Select(unhandled => SandboxRunner.Run(pathToCompiler, unhandled)).ToList();
					Console.WriteLine($"Results: [{string.Join(", ", results.Select(r => r.Verdict))}]");
					client.SendResults(results);
				}
				Thread.Sleep(sleep);
			}
			// ReSharper disable once FunctionNeverReturns
		}

		private static void SelfCheck(string pathToCompiler)
		{
			var res = SandboxRunner.Run(pathToCompiler,
				new FileRunnerSubmition
				{
					Id = Utils.NewNormalizedGuid(),
					NeedRun = true,
					Code = "class C { static void Main(){ System.Console.WriteLine(\"Привет мир!\");}}"
				});
			Console.WriteLine(res);
		}
	}
}