using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using RunCsJob.Api;

namespace RunCsJob
{
	internal class RunCsJobProgram
	{
		private readonly string address;
		private readonly string token;
		private readonly TimeSpan sleep;
		private readonly int jobsToRequest;

		public RunCsJobProgram()
		{
			address = ConfigurationManager.AppSettings["submissionsUrl"];
			token = ConfigurationManager.AppSettings["runnerToken"];
			sleep = TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings["sleepSeconds"] ?? "1"));
			jobsToRequest = int.Parse(ConfigurationManager.AppSettings["jobsToRequest"] ?? "5");
		}

		public static void Main(string[] args)
		{
			if (args.Contains("--selfcheck"))
				SelfCheck();
			else
				new RunCsJobProgram().Run();
		}

		private void Run()
		{
			AppDomain.MonitoringIsEnabled = true;
			Console.WriteLine("Listen {0}", address);
			var client = new Client(address, token);
			MainLoop(client);
		}

		private void MainLoop(Client client)
		{
			while (true)
			{
				var newUnhandled = client.TryGetSubmissions(jobsToRequest).Result;
				Console.WriteLine("Received {0} submissions: [{1}]", newUnhandled.Count, string.Join(", ", newUnhandled.Select(s => s.Id)));

				if (newUnhandled.Any())
				{
					var results = newUnhandled.Select(SandboxRunner.Run).ToList();
					Console.WriteLine("Results: [{0}]", string.Join(", ", results.Select(r => r.Verdict)));
					client.SendResults(results);
				}
				Thread.Sleep(sleep);
			}
			// ReSharper disable once FunctionNeverReturns
		}

		private static void SelfCheck()
		{
			var res = SandboxRunner.Run(new RunnerSubmition()
			{
				Id = Guid.NewGuid().ToString("N"),
				NeedRun = true,
				Code = "class C { static void Main(){ System.Console.WriteLine(\"Привет мир!\");}}"
			});
			Console.WriteLine(res);
		}
	}
}
