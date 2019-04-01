using Graphite.Web;
using System.Web.Configuration;
using Metrics;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using uLearn.Web;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(GraphiteMetricsPipeStart), "PreStart")]

namespace uLearn.Web
{
	public class GraphiteMetricsPipeStart
	{
		public static void PreStart()
		{
			var connectionString = WebConfigurationManager.ConnectionStrings["statsd"]?.ConnectionString;
			var isGraphiteSendingEnabled = !string.IsNullOrEmpty(connectionString);
			
			if (!isGraphiteSendingEnabled)
				return;
			
			var config = StatsdConfiguration.CreateFrom(connectionString);

			// Make sure MetricsPipe handles BeginRequest and EndRequest
			DynamicModuleUtility.RegisterModule(typeof(MetricsPipeStartupModule));

			MetricsPipeStartupModule.Settings.ReportRequestTime = true;
			MetricsPipeStartupModule.Settings.RequestTimePrefix = MetricSender.BuildKey(config.Prefix, "web", "request.time");
		}
	}
}