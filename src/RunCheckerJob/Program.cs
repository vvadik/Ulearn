using System;
using System.IO;
using System.Threading;
using log4net;
using System.Linq;
using System.Reflection;
using System.Xml;
using log4net.Config;
using Ulearn.Core;
using Ulearn.Core.RunCheckerJobApi;

namespace RunCheckerJob
{
	public class Program : ProgramBase
	{
		private const string serviceName = "runcheckerjob";
		private readonly DockerSandboxRunner sandboxRunner;
		protected override Language[] SupportedLanguages { get; } = {Language.JavaScript};

		public static void Main(string[] args)
		{
			ConfigureLog4Net();
			var isSelfCheck = args.Contains("--selfcheck");

			var program = new Program();
			if (isSelfCheck)
				program.SelfCheck();
			else
				program.Run();
		}
		
		private static void ConfigureLog4Net()
		{
			var log4NetConfig = new XmlDocument();
			log4NetConfig.Load(File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
			var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
			XmlConfigurator.Configure(repo, log4NetConfig["configuration"]["log4net"]);
		}
		
		private Program(ManualResetEvent externalShutdownEvent = null)
			: base(serviceName, externalShutdownEvent)
		{
			sandboxRunner = new DockerSandboxRunner();
		}
		
		protected override ISandboxRunnerClient SandboxRunnerClient => sandboxRunner;

		private void SelfCheck()
		{
			new SelfChecker(sandboxRunner).JsSelfCheck();
		}
	}
}