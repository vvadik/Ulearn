using System.IO;
using System.Linq;
using log4net.Config;

namespace RunCsJob
{
	public class Program
	{
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
			
			var program = new RunCsJobProgramBase(сompilerDirectory);
			if (isSelfCheck)
				program.SelfCheck();
			else
				program.Run();
		}
	}
}