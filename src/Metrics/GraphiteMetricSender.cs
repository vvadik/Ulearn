using System;
using Graphite;
using log4net;

namespace Metrics
{
    public class GraphiteMetricSender
    {
	    private readonly ILog log = LogManager.GetLogger(typeof(GraphiteMetricSender));

	    private readonly string service;
	    private readonly string machineName;

	    public GraphiteMetricSender(string service)
	    {
		    this.service = service;
		    machineName = Environment.MachineName.Replace(".", "_").ToLower();
	    }

	    private string BuildKey(string key)
	    {
		    return $"{service}.{machineName}.{key}";
	    }

		public void SendCount(string key, int value = 1, float sampling = 1)
		{
			var builtKey = BuildKey(key);
			log.Info($"Send count metric {builtKey}, value {value}");
		    try
		    {
			    MetricsPipe.Current.Count(builtKey, value, sampling);
		    }
		    catch (Exception e)
		    {
			    log.Warn($"Can't send count metric {builtKey}, value {value}", e);
		    }
	    }

	    public void SendTiming(string key, int value)
	    {
		    var builtKey = BuildKey(key);
			log.Info($"Send timing metric {builtKey}, value {value}");
			try
			{ 
				MetricsPipe.Current.Timing(builtKey, value);
			}
			catch (Exception e)
			{
				log.Warn($"Can't send timing metric {builtKey}, value {value}", e);
			}
		}

	    public void SendGauge(string key, int value)
	    {
		    var builtKey = BuildKey(key);
			log.Info($"Send gauge metric {builtKey}, value {value}");
			try
			{ 
				MetricsPipe.Current.Gauge(builtKey, value);
			}
			catch (Exception e)
			{
				log.Warn($"Can't send gauge metric {builtKey}, value {value}", e);
			}
		}

	    public void SendRaw(string key, int value)
	    {
		    var builtKey = BuildKey(key);
			log.Info($"Send raw metric {builtKey}, value {value}");
			try
			{ 
				MetricsPipe.Current.Raw(builtKey, value);
			}
			catch (Exception e)
			{
				log.Warn($"Can't send raw metric {builtKey}, value {value}", e);
			}
		}
	}
}
