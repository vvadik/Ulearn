using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

namespace CourseToolHotReloader.Log
{
	public class Logger
	{
		private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static ILog Log => log;

		public static void InitLogger()
		{
			var repository = LogManager.GetRepository(Assembly.GetCallingAssembly());

			var fileInfo = new FileInfo($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\log4net.config");

			XmlConfigurator.Configure(repository, fileInfo);
		}
	}
}