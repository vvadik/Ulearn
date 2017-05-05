using System.Configuration;
using log4net;

namespace XQueueWatcher
{
	class Program
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Program));

		static void Main(string[] args)
		{
			var baseUrl = ConfigurationManager.AppSettings["ulearn.xQueueWatcher.baseUrl"] ?? "";
			if (string.IsNullOrEmpty(baseUrl))
			{
				log.Error("Invalid baseUrl for xqueue watcher, can\'t start. Set correct value for app setting ulearn.xQueueWatcher.baseUrl");
				return;
			}
			var queueName = ConfigurationManager.AppSettings["ulearn.xQueueWatcher.queueName"] ?? "";
			var username = ConfigurationManager.AppSettings["ulearn.xQueueWatcher.username"] ?? "";
			var password = ConfigurationManager.AppSettings["ulearn.xQueueWatcher.password"] ?? "";

			var watcher = new Watcher(baseUrl, queueName, username, password);
			watcher.Loop().Wait();
		}
	}
}
