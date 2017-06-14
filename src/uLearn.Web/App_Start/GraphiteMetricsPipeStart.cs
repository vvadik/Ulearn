using System.Diagnostics;
using Graphite.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(uLearn.Web.App_Start.GraphiteMetricsPipeStart), "PreStart")]

namespace uLearn.Web.App_Start
{
	public class GraphiteMetricsPipeStart
	{
		public static void PreStart()
		{
			Trace.WriteLine("GraphiteMetricsPipeStart.PreStart");
			// Make sure MetricsPipe handles BeginRequest and EndRequest
			DynamicModuleUtility.RegisterModule(typeof(MetricsPipeStartupModule));

			MetricsPipeStartupModule.Settings.ReportRequestTime = true;
			MetricsPipeStartupModule.Settings.RequestTimePrefix = "request.time";
		}
	}
}