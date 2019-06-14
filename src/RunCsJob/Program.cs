using System;
using System.IO;
using System.Linq;
using System.Threading;
using log4net;
using log4net.Config;
using RunCheckerJob;
using Ulearn.Core;
using Ulearn.Core.RunCheckerJobApi;

namespace RunCsJob
{
	public class Program : ProgramBase
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Program));
		private const string serviceName = "runscjob";
		private readonly CsSandboxRunnerClient csSandboxRunnerClient;
		
		public static void Main(string[] args)
		{
			XmlConfigurator.Configure();

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

		private Program(DirectoryInfo сompilerDirectory, ManualResetEvent externalShutdownEvent = null)
			: base(serviceName, externalShutdownEvent)
		{
			if (!сompilerDirectory.Exists)
			{
				log.Error($"Не найдена папка с компиляторами: {сompilerDirectory}");
				Environment.Exit(1);
			}
			log.Info($"Путь до компиляторов: {сompilerDirectory}");

			CsSandboxRunnerSettings.MsBuildSettings = new MsBuildSettings {CompilerDirectory = сompilerDirectory};
			csSandboxRunnerClient = new CsSandboxRunnerClient();
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