using Vostok.Hosting.Setup;
using Vostok.Logging.Abstractions;

namespace Ulearn.Common.Api
{
	internal static class VostokHostingEnvironmentBuilderExtensions
	{
		private static class KonturVostokConstants
		{
			//public const string DevEnvironment = "dev";
			//public const string CloudEnvironment = "cloud";
			//public const string ProdEnvironment = "prod";
			//public const string DefaultEnvironment = "default";
			//public const string DefaultEnvironmentNamePath = "vostok/environment/DefaultEnvironmentName";
			//public const string HerculesSinkTopologyPath = "topology/hercules/gate.prod";
			public const string HerculesSinkServiceDiscoveryName = "Hercules.Gate";
			//public const string HerculesStreamApiServiceDiscoveryName = "Hercules.StreamApi";
			//public const string HerculesSettingsPath = "vostok/hercules";
			public const string ZooKeeperTopologyPath = "topology/zookeeper-global";
			public const string TracingStreamNamePath = "vostok/tracing/StreamName";
			public const string TracingPublicApiKeyPath = "vostok/tracing/PublicApiKey";
			public const string LoggingStreamNamePath = "vostok/logging/StreamName";
			public const string LoggingPublicApiKeyPath = "vostok/logging/PublicApiKey";
			public const string MetricsPublicApiKeyPath = "vostok/metrics/PublicApiKey";
		}

		public static IVostokHostingEnvironmentBuilder SetupForKontur(this IVostokHostingEnvironmentBuilder builder)
		{
			return builder
				.EnableClusterConfig()
				.SetupHerculesSink(ConfigureHerculesSink)
				//.SetupLog(ConfigureHerculesLog) не нужно устанавливать параметры по умолчанию, т.к. они переопределяются в EnvironmentSetup.
				.SetupMetrics(ConfigureMetrics)
				.SetupTracer(ConfigureTracer)
				.SetupZooKeeperClient(ConfigureZooKeeper)
				;
		}

		private static void ConfigureHerculesSink(IVostokHerculesSinkBuilder builder, IVostokHostingEnvironmentSetupContext context)
		{
			builder.SetServiceDiscoveryTopology("default", KonturVostokConstants.HerculesSinkServiceDiscoveryName);
		}

		// Set default (common) stream и apiKey
		private static void ConfigureHerculesLog(IVostokCompositeLogBuilder builder, IVostokHostingEnvironmentSetupContext context)
		{
			var logsStream = context.ClusterConfigClient.Get(KonturVostokConstants.LoggingStreamNamePath)?.Value;
			if (logsStream == null)
			{
				context.Log.Warn("Could not find logging Hercules stream in ClusterConfig.");
				return;
			}

			builder.SetupHerculesLog(
				herculesLog => herculesLog
					.SetStream(logsStream)
					.SetApiKeyProvider(() => context.ClusterConfigClient.Get(KonturVostokConstants.LoggingPublicApiKeyPath)?.Value));
		}

		private static void ConfigureMetrics(IVostokMetricsBuilder builder, IVostokHostingEnvironmentSetupContext context)
		{
			builder.SetupHerculesMetricEventSender(
				herculesSender => herculesSender
					.SetApiKeyProvider(() => context.ClusterConfigClient.Get(KonturVostokConstants.MetricsPublicApiKeyPath)?.Value)
			);
#if DEBUG
			builder.SetupLoggingMetricEventSender();
#endif
		}

		private static void ConfigureTracer(IVostokTracerBuilder builder, IVostokHostingEnvironmentSetupContext context)
		{
			var tracingStream = context.ClusterConfigClient.Get(KonturVostokConstants.TracingStreamNamePath)?.Value;
			if (tracingStream == null)
			{
				context.Log.Warn("Could not find tracing Hercules stream in ClusterConfig.");
				return;
			}

			builder.SetupHerculesSpanSender(
				sender => sender
					.SetStream(tracingStream)
					.SetApiKeyProvider(() => context.ClusterConfigClient.Get(KonturVostokConstants.TracingPublicApiKeyPath)?.Value)
			);
		}
		
		private static void ConfigureZooKeeper(IVostokZooKeeperClientBuilder builder, IVostokHostingEnvironmentSetupContext context)
		{
			var topology = context.ClusterConfigClient.Get(KonturVostokConstants.ZooKeeperTopologyPath);
			if (topology == null)
			{
				context.Log.Warn("Could not find ZooKeeper topology in ClusterConfig.");
				return;
			}

			builder.SetClusterConfigTopology(KonturVostokConstants.ZooKeeperTopologyPath);
		}
	}
}