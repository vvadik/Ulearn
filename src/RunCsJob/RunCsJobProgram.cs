using System;
using System.Configuration;
using System.Linq;
using System.Threading;

namespace RunCsJob
{
	internal class RunCsJobProgram
	{
		private readonly string address;
		private readonly string token;

		public RunCsJobProgram()
		{
			address = ConfigurationManager.AppSettings["submissionsUrl"];
			token = ConfigurationManager.AppSettings["runnerToken"];
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

		private static void MainLoop(Client client)
		{
			while (true)
			{
				var newUnhandled = client.TryGetSubmissions(10).Result;
				foreach (var submission in newUnhandled)
					Console.WriteLine("Received " + submission);
				var results = newUnhandled.Select(SandboxRunner.Run).ToList();
				foreach (var res in results)
					Console.WriteLine("Result " + res);
				if (results.Any())
					client.SendResults(results);
				Thread.Sleep(1000);
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
