using System.Threading.Tasks;
using Ulearn.Common.Api;
using Vostok.Hosting;

namespace Notifications
{
	public class EntryPoint
	{
		public static async Task Main(string[] args)
		{
			var isOneTimeSend = args.Length > 0 && args[0] == "send";
			var application = new NotificationsApplication(isOneTimeSend);
			var setupBuilder = new EnvironmentSetupBuilder("notifications", args);
			var hostSettings = new VostokHostSettings(application, setupBuilder.EnvironmentSetup);
			var host = new VostokHost(hostSettings);
			await host.WithConsoleCancellation().RunAsync();
		}
	}
}