using System;
using System.Threading;
using log4net;
using Ulearn.Core.RunCheckerJobApi;

namespace RunCheckerJob
{
	public class RunCheckerJobProgram : RunCheckerJobProgramBase
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(RunCheckerJobProgram));
		private const string serviceName = "runcheckerjob";
		private readonly CheckerSandboxRunnerClient sandboxRunnerClient;
		
		public RunCheckerJobProgram(ManualResetEvent externalShutdownEvent = null)
			: base(serviceName, externalShutdownEvent)
		{
			sandboxRunnerClient = new CheckerSandboxRunnerClient(new DockerSandboxRunnerSettings(serviceName, "js-sandbox"));
		}

		protected override ISandboxRunnerClient SandboxRunnerClient => sandboxRunnerClient;
		
		// TODO собрать тестовый архив и реализовать
		internal void SelfCheck()
		{
			throw new NotImplementedException();
			// var res = SandboxRunner.Run(new FileRunnerSubmission
			// {
			// 	Id = Utils.NewNormalizedGuid(),
			// 	NeedRun = true,
			// 	Code = "console.log('Привет мир')",
			// }, Settings);
			// log.Info(res);
		}
	}
}