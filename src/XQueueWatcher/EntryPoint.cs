using System.Threading.Tasks;
using Ulearn.Common.Api;
using Vostok.Hosting;

namespace XQueueWatcher
{
	public class EntryPoint
	{
		public static async Task Main(string[] args)
		{
			var application = new XQueueWatcherApplication();
			var setupBuilder = new EnvironmentSetupBuilder("xqueuewatcher", args);
			var hostSettings = new VostokHostSettings(application, setupBuilder.EnvironmentSetup);
			var host = new VostokHost(hostSettings);
			await host.WithConsoleCancellation().RunAsync();
		}
	}
}