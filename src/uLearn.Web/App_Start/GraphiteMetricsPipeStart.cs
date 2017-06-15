using Graphite.Web;
using Metrics;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(uLearn.Web.App_Start.GraphiteMetricsPipeStart), "PreStart")]

namespace uLearn.Web.App_Start
{
	public class GraphiteMetricsPipeStart
	{
		public static void PreStart()
		{
			// Make sure MetricsPipe handles BeginRequest and EndRequest
			DynamicModuleUtility.RegisterModule(typeof(MetricsPipeStartupModule));

			MetricsPipeStartupModule.Settings.ReportRequestTime = true;
			MetricsPipeStartupModule.Settings.RequestTimePrefix = GraphiteMetricSender.BuildKey("web", "request.time");
		}
	}
}