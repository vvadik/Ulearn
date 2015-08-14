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

		public static void Main()
		{
			new RunCsJobProgram().Run();
		}

		private void Run()
		{
			AppDomain.MonitoringIsEnabled = true;

			Console.WriteLine("Listen {0}", address);

			var client = new Client(address, token);
			while (true)
			{
				var newUnhandled = client.TryGetSubmissions(10).Result;
				foreach (var submission in newUnhandled)
					Console.WriteLine("Received " + submission);
				var results = newUnhandled.Select(HandleSubmission).ToList();
				foreach (var res in results)
				{
					Console.WriteLine(res);
				}
				if (results.Any())
					client.SendResults(results);
				Thread.Sleep(1000);
			}
			// ReSharper disable once FunctionNeverReturns
		}

		private static RunningResults HandleSubmission(InternalSubmissionModel submission)
		{
			RunningResults result;
			try
			{
				result = new SandboxRunner(submission).Run();
			}
			catch (Exception ex)
			{
				result = new RunningResults
				{
					Id = submission.Id,
					Verdict = Verdict.SandboxError,
					Error = ex.ToString()
				};
			}
			return result;
		}
	}
}
