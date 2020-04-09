using Vostok.Datacenters.Kontur.Helpers;
using Vostok.Hosting.Setup;
using Vostok.Logging.Abstractions;
using Vostok.Telemetry.Kontur;

namespace Ulearn.Common.Api
{
	internal static class VostokHostingEnvironmentBuilderExtensions
	{
		public static IVostokHostingEnvironmentBuilder SetupForKontur(this IVostokHostingEnvironmentBuilder builder)
		{
			return builder
				.EnableClusterConfig()
				.SetupHerculesSink(ConfigureHerculesSink)
				//.SetupLog(ConfigureHerculesLog) не нужно устанавливать параметры по умолчанию, т.к. они переопределяются в EnvironmentSetup.
				.SetupMetrics(ConfigureMetrics)
				.SetupTracer(ConfigureTracer)
				.SetupZooKeeperClient(ConfigureZooKeeper)
				.SetupDatacenters(ConfigureDatacenters)
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
		
		private static void ConfigureDatacenters(IVostokDatacentersBuilder builder, IVostokHostingEnvironmentSetupContext context)
			=> builder
				.SetActiveDatacentersProvider(
					KonturActiveDatacentersProvider.Get(context.Log, context.ClusterConfigClient, context.ConfigurationProvider))
				.SetDatacenterMapping(
					KonturDatacenterMappingProvider.Get(context.Log, context.ClusterConfigClient, context.ConfigurationProvider));

	}
}