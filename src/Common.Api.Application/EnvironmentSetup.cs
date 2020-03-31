using System;
using System.IO;
using Serilog;
using Serilog.Events;
using Ulearn.Common.Api.Helpers;
using Ulearn.Core.Configuration;
using Vostok.Configuration;
using Vostok.Configuration.Abstractions;
using Vostok.Configuration.Sources;
using Vostok.Configuration.Sources.CommandLine;
using Vostok.Configuration.Sources.Json;
using Vostok.Hosting.Setup;
using Vostok.Logging.Serilog;
using Web.Api.Configuration;

namespace Ulearn.Common.Api
{
	public class EnvironmentSetupBuilder
	{
		private readonly string application;
		private readonly string[] commandLineArguments;

		public EnvironmentSetupBuilder(string application, string[] commandLineArguments)
		{
			this.application = application;
			this.commandLineArguments = commandLineArguments;
		}

		public void EnvironmentSetup(IVostokHostingEnvironmentBuilder builder)
		{
			var configurationProvider = new ConfigurationProvider(new ConfigurationProviderSettings());
			var configurationSource = GetConfigurationSource();
			configurationProvider.SetupSourceFor<UlearnConfiguration>(configurationSource);
			var ulearnConfiguration = configurationProvider.Get<UlearnConfiguration>();
// ReSharper disable once PossibleInvalidOperationException
			var port = ulearnConfiguration.Port.Value;

			builder
				.SetupApplicationIdentity(identityBuilder => identityBuilder
					.SetProject("ulearn")
					.SetApplication(application)
					.SetEnvironment("default")
					.SetInstance(Environment.MachineName))
				.SetupConfiguration(configurationBuilder => configurationBuilder.AddSource(configurationSource))
				.DisableClusterConfig()
				.DisableServiceBeacon()
				.DisableHercules()
				.DisableZooKeeper()
				.DisableClusterConfigLocalSettings()
				.DisableClusterConfigRemoteSettings()
				.SetPort(port)
				.SetBaseUrlPath(ulearnConfiguration.BaseUrl)
				.SetupLog((logBuilder, context) =>
				{
					var loggerConfiguration = GetSerilogLoggerConfiguration(ulearnConfiguration);
					var hostLog = new SerilogLog(loggerConfiguration.CreateLogger());
					logBuilder.AddLog(hostLog);
				});
		}

		private static LoggerConfiguration GetSerilogLoggerConfiguration(UlearnConfiguration ulearnConfiguration)
		{
			var loggerConfiguration = new LoggerConfiguration().MinimumLevel.Information();
			if (ulearnConfiguration.HostLog.Console)
			{
				loggerConfiguration = loggerConfiguration
					.WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff} {Level:u3} [{Thread}] {Message:l}{NewLine}{Exception}", restrictedToMinimumLevel: LogEventLevel.Information);
			}

			var pathFormat = ulearnConfiguration.HostLog.PathFormat;
			if (!string.IsNullOrEmpty(pathFormat))
			{
				var minimumLevelString = ulearnConfiguration.HostLog.MinimumLevel ?? "debug";
				var dbMinimumLevelString = ulearnConfiguration.HostLog.DbMinimumLevel ?? "";
				if (!Enum.TryParse(minimumLevelString, true, out LogEventLevel minimumLevel) || !Enum.IsDefined(typeof(LogEventLevel), minimumLevel))
					minimumLevel = LogEventLevel.Debug;
				if (!Enum.TryParse(dbMinimumLevelString, true, out LogEventLevel dbMinimumLevel) || !Enum.IsDefined(typeof(LogEventLevel), dbMinimumLevel))
					dbMinimumLevel = minimumLevel;
				if (Path.IsPathRooted(pathFormat))
				{
					var directory = Path.GetDirectoryName(pathFormat);
					var fileName = Path.GetFileName(pathFormat);
					pathFormat = Path.Combine(directory, ulearnConfiguration.GraphiteServiceName, fileName);
				}

				loggerConfiguration = loggerConfiguration
					.WriteTo.RollingFile(
						pathFormat,
						outputTemplate: "{Timestamp:HH:mm:ss.fff} {Level:u3} [{Thread}] {Message:l}{NewLine}{Exception}",
						restrictedToMinimumLevel: minimumLevel,
						fileSizeLimitBytes: 4 * 1073741824L
					);

				if (dbMinimumLevel != minimumLevel)
				{
					loggerConfiguration = loggerConfiguration.Filter.ByIncludingOnly(le =>
						le.Level >= dbMinimumLevel || !LogEventHelpers.IsDbSource(le));
				}
			}

			return loggerConfiguration;
		}

		private IConfigurationSource GetConfigurationSource()
		{
			var configurationSource = new CommandLineSource(commandLineArguments)
				.CombineWith(new JsonFileSource("appsettings.json"))
				.CombineWith(new JsonFileSource("appsettings." + (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production") + ".json"));
			var environmentName = Environment.GetEnvironmentVariable("UlearnEnvironmentName");
			if (environmentName != null && environmentName.ToLower().Contains("local"))
				configurationSource.CombineWith(new JsonFileSource("appsettings.local.json"));
			return configurationSource;
		}
	}
}