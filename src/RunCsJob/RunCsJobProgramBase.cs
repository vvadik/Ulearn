using System;
using System.IO;
using System.Threading;
using log4net;
using RunCheckerJob;
using Ulearn.Core;
using Ulearn.Core.RunCheckerJobApi;

namespace RunCsJob
{
	public class RunCsJobProgramBase : RunCheckerJobProgramBase
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(RunCsJobProgramBase));
		private const string serviceName = "runscjob";
		private readonly CsSandboxRunnerClient csSandboxRunnerClient;

		public RunCsJobProgramBase(DirectoryInfo сompilerDirectory, ManualResetEvent externalShutdownEvent = null)
			: base(serviceName, externalShutdownEvent)
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