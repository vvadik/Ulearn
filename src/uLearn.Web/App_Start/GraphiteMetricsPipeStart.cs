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
		private static bool IsGraphiteSendingEnabled => !string.IsNullOrEmpty(WebConfigurationManager.ConnectionStrings["statsd"]?.ConnectionString);

		public static void PreStart()
		{
			if (!IsGraphiteSendingEnabled)
				return;

			// Make sure MetricsPipe handles BeginRequest and EndRequest
			DynamicModuleUtility.RegisterModule(typeof(MetricsPipeStartupModule));

			MetricsPipeStartupModule.Settings.ReportRequestTime = true;
			MetricsPipeStartupModule.Settings.RequestTimePrefix = GraphiteMetricSender.BuildKey("web", "request.time");
		}
	}
}