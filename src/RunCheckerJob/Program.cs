using System;
using System.Configuration;
using System.IO;
using System.Threading;
using log4net;
using System.Linq;
using System.Reflection;
using log4net.Config;
using Ulearn.Core.RunCheckerJobApi;

namespace RunCheckerJob
{
	public class Program : ProgramBase
	{
		private const string serviceName = "runcheckerjob";
		private readonly DockerSandboxRunner sandboxRunner;
		
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
			var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
			var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			XmlConfigurator.Configure(logRepository, new FileInfo(config.FilePath));
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