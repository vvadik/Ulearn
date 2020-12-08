using System;
using System.IO;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using log4net;
using log4net.Config;
using RunCheckerJob;
using Ulearn.Core;
using Ulearn.Core.Logging;
using Ulearn.Core.RunCheckerJobApi;

namespace RunCsJob
{
	public class Program : ProgramBase
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Program));
		private const string serviceName = "runcsjob";
		private readonly CsSandboxRunnerClient csSandboxRunnerClient;

		public static void Main(string[] args)
		{
			XmlConfigurator.Configure();
			LoggerSetup.SetupForLog4Net();
			DirectoryInfo сompilerDirectory = null;
			if (args.Any(x => x.StartsWith("-p:")))
			{
				var path = args.FirstOrDefault(x => x.StartsWith("-p:"))?.Substring(3);
				if (path != null)
					сompilerDirectory = new DirectoryInfo(path);
			}

			var isSelfCheck = args.Contains("--selfcheck");

			var program = new Program(сompilerDirectory);
			if (isSelfCheck)
				program.SelfCheck();
			else
				program.Run();
		}

		private Program([CanBeNull] DirectoryInfo сompilerDirectory, ManualResetEvent externalShutdownEvent = null)
			: base(serviceName, externalShutdownEvent)
		{
			if (сompilerDirectory != null)
				CsSandboxRunnerSettings.MsBuildSettings.CompilerDirectory = сompilerDirectory;
			if (!CsSandboxRunnerSettings.MsBuildSettings.CompilerDirectory.Exists)
			{
				log.Error($"Не найдена папка с компиляторами: {сompilerDirectory}");
				Environment.Exit(1);
			}

			log.Info($"Путь до компиляторов: {сompilerDirectory}");
			csSandboxRunnerClient = new CsSandboxRunnerClient();
		}

		protected override ISandboxRunnerClient SandboxRunnerClient => csSandboxRunnerClient;

		private void SelfCheck()
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