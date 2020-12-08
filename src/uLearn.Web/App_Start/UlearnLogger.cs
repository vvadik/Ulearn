using System.Web;
using log4net.Config;
using Serilog;
using Ulearn.Core.Logging;

namespace uLearn.Web
{
	public static class UlearnLogger
	{
		public static void ConfigureLogging()
		{
			XmlConfigurator.Configure();
			log4net.GlobalContext.Properties["user"] = new HttpContextUserNameProvider();
			log4net.GlobalContext.Properties["address"] = new HttpContextAddressProvider();
			LoggerSetup.SetupForLog4Net();
		}
	}

	public class HttpContextUserNameProvider
	{
		public override string ToString()
		{
			var context = HttpContext.Current;
			if (context?.User != null && context.User.Identity.IsAuthenticated)
				return context.User.Identity.Name;
			return "(unknown user)";
		}
	}

	public class HttpContextAddressProvider
	{
		private const string xRealIpHeaderName = "X-Real-IP";

		public override string ToString()
		{
			var context = HttpContext.Current;
			var realIp = context?.Request.Headers[xRealIpHeaderName];
			if (!string.IsNullOrEmpty(realIp))
				return realIp;
			return context?.Request.UserHostAddress ?? "";
		}
	}
}