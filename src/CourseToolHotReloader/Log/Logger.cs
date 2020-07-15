using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
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

			var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "log4net.config");

			ChangeLogsOutputDirectory(path);

			var fileInfo = new FileInfo(path);

			XmlConfigurator.Configure(repository, fileInfo);
		}

		private static void ChangeLogsOutputDirectory(string path)
		{
			var xDoc = new XmlDocument();
			xDoc.Load(path);
			
			var xRoot = xDoc.DocumentElement;
			foreach (var xnode in xRoot.Cast<XmlNode>().Where(xnode => xnode.Name.Equals("appender", StringComparison.OrdinalIgnoreCase)))
				xnode["file"].SetAttribute("value", NewPath(xnode["file"].Attributes["value"].Value));
			
			xDoc.Save(path);
		}

		private static string NewPath(string oldPath)
		{
			var fileName= Path.GetFileName(oldPath);
			return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "logs/fileName");
		}
	}
}