using System;

namespace Metrics
{
	public class ServiceKeepAliver
	{
		private readonly GraphiteMetricSender sender;
		public DateTime LastPingTime { get; private set; }

		public ServiceKeepAliver(GraphiteMetricSender sender)
		{
			this.sender = sender;
			LastPingTime = DateTime.MinValue;
		}
		
		public ServiceKeepAliver(string serviceName)
			:this(new GraphiteMetricSender(serviceName))
		{
		}

		public void Ping()
		{
			sender.SendCount("keep_alive.ping");
			LastPingTime = DateTime.Now;
		}

		public void Ping(TimeSpan minDelay)
		{
			var currentTime = DateTime.Now;
			if (currentTime - LastPingTime > minDelay)
				Ping();
		}
	}
}