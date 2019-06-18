using System.Threading;
using log4net;
using System.Linq;
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
		
		private void SelfCheck()
		{
			new SelfChecker(sandboxRunner).JsSelfCheck();
		}
	}
}