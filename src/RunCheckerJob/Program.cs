using System;
using System.Threading;
using log4net;
using System.Linq;
using RunCheckerJob.Js;
using Ulearn.Core.RunCheckerJobApi;

namespace RunCheckerJob
{
	public class Program : ProgramBase
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Program));
		private const string serviceName = "runcheckerjob";
		private readonly DockerSandboxRunner sandboxRunner;
		
		public static void Main(string[] args)
		{
			var isSelfCheck = args.Contains("--selfcheck");

			var program = new Program();
			if (isSelfCheck)
				program.SelfCheck();
			else
				program.Run();
		}
		
		private Program(ManualResetEvent externalShutdownEvent = null)
			: base(serviceName, externalShutdownEvent)
		{
			sandboxRunner = new DockerSandboxRunner();
		}
		
		protected override ISandboxRunnerClient SandboxRunnerClient => sandboxRunner;
		
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