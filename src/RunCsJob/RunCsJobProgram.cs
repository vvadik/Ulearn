using System;
using System.IO;
using System.Threading;
using log4net;
using RunCheckerJob;
using RunCheckerJob.Api;
using Ulearn.Core;

namespace RunCsJob
{
	public class RunCsJobProgram : RunCheckerJobProgram
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(RunCsJobProgram));
		private readonly CsSandboxRunnerClient csSandboxRunnerClient;

		public RunCsJobProgram(DirectoryInfo сompilerDirectory, ManualResetEvent externalShutdownEvent = null)
			: base("runscjob", externalShutdownEvent)
		{
			if (!сompilerDirectory.Exists)
			{
				log.Error($"Не найдена папка с компиляторами: {сompilerDirectory}");
				Environment.Exit(1);
			}
			log.Info($"Путь до компиляторов: {сompilerDirectory}");

			var csSandboxRunnerSettings = new CsSandboxRunnerSettings(сompilerDirectory);
			csSandboxRunnerClient = new CsSandboxRunnerClient(csSandboxRunnerSettings);
		}

		protected override ISandboxRunnerClient SandboxRunnerClient => csSandboxRunnerClient;

		internal void SelfCheck()
		{
			var res = SandboxRunnerClient.Run(new FileRunnerSubmission
			{
				Id = Utils.NewNormalizedGuid(),
				NeedRun = true,
				Code = "class C { static void Main(){ System.Console.WriteLine(\"Привет мир!\");}}"
			});
			log.Info(res);
		}
	}
}