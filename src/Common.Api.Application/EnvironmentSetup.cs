using System;
using System.IO;
using Ulearn.Core.Configuration;
using Vostok.Configuration;
using Vostok.Configuration.Abstractions;
using Vostok.Configuration.Sources;
using Vostok.Configuration.Sources.CommandLine;
using Vostok.Configuration.Sources.Json;
using Vostok.Hosting.Kontur;
using Vostok.Hosting.Setup;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Abstractions.Values;
using Vostok.Logging.File.Configuration;
using Vostok.Logging.Formatting;

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
			var environment = ulearnConfiguration.Environment ?? "dev";

			if (!ulearnConfiguration.DisableKonturServices)
				builder = builder.SetupForKontur();

			builder.SetupApplicationIdentity(identityBuilder => identityBuilder
					.SetProject("ulearn")
					.SetApplication(application)
					.SetEnvironment(environment)
					.SetInstance(Environment.MachineName.Replace(".", "_").ToLower()))
				.SetupConfiguration(configurationBuilder => configurationBuilder.AddSecretSource(configurationSource))
				.SetPort(port)
				.SetBaseUrlPath(ulearnConfiguration.BaseUrl)
				.DisableServiceBeacon()
				.SetupHerculesSink(sinkBuilder => SetupHerculesSink(sinkBuilder, ulearnConfiguration))
				.SetupLog((logBuilder, context) => SetupLog(logBuilder, ulearnConfiguration))
				;

			if (ulearnConfiguration.ForceHttps ?? false)
				builder.SetHttpsScheme();
		}

		private static void SetupHerculesSink(IVostokHerculesSinkBuilder sinkBuilder, UlearnConfiguration ulearnConfiguration)
		{
			if (ulearnConfiguration.Hercules == null || string.IsNullOrEmpty(ulearnConfiguration.Hercules.ApiKey))
			{
				sinkBuilder.Disable();
				return;
			}
			sinkBuilder.SetApiKeyProvider(() => ulearnConfiguration.Hercules.ApiKey);
		}

		private static void SetupLog(IVostokCompositeLogBuilder logBuilder, UlearnConfiguration ulearnConfiguration)
		{
			var logTemplate = OutputTemplate.Parse("{Timestamp:HH:mm:ss.fff} {Level} {traceContext:w}{operationContext:w}{sourceContext:w}{Message}{NewLine}{Exception}");
			logBuilder.SetupConsoleLog(consoleLogBuilder => consoleLogBuilder
				.CustomizeLog(lb => lb.WithMinimumLevel(LogLevel.Info))
				.CustomizeSettings(settings => { settings.OutputTemplate = logTemplate; }));

			var pathFormat = ulearnConfiguration.HostLog.PathFormat;
			if (!string.IsNullOrEmpty(pathFormat))
			{
				var minimumLevelString = ulearnConfiguration.HostLog.MinimumLevel ?? "debug";
				var dbMinimumLevelString = ulearnConfiguration.HostLog.DbMinimumLevel ?? "";
				if (!TryParseLogLevel(minimumLevelString, out var minimumLevel))
					minimumLevel = LogLevel.Debug;
				if (!TryParseLogLevel(dbMinimumLevelString, out var dbMinimumLevel))
					dbMinimumLevel = minimumLevel;
				pathFormat = pathFormat.Replace("{Date}", "{RollingSuffix}"); // Для совместимости с настройками appsettings.json, написанными для серилога
				if (Path.IsPathRooted(pathFormat))
				{
					var directory = Path.GetDirectoryName(pathFormat);
					var fileName = Path.GetFileName(pathFormat);
					pathFormat = Path.Combine(directory, ulearnConfiguration.GraphiteServiceName, fileName);
				}

				logBuilder.SetupFileLog(fileLogBuilder => fileLogBuilder
					.CustomizeLog(lb =>
					{
						var customized = lb.WithMinimumLevel(minimumLevel);
						if (dbMinimumLevel != minimumLevel)
							customized = customized.SelectEvents(le => le.Level >= dbMinimumLevel || !IsDbSource(le));
						return customized;
					})
					.SetSettingsProvider(() => new FileLogSettings
					{
						FilePath = pathFormat,
						RollingStrategy = new RollingStrategyOptions
						{
							MaxFiles = 0,
							Type = RollingStrategyType.Hybrid,
							Period = RollingPeriod.Day,
							MaxSize = 4 * 1073741824L
						},
						OutputTemplate = logTemplate
					}));

				logBuilder.SetupHerculesLog(herculesLogBuilder =>
				{
					herculesLogBuilder.SetStream(ulearnConfiguration.Hercules.Stream);
					herculesLogBuilder.SetApiKeyProvider(() => ulearnConfiguration.Hercules.ApiKey);
					herculesLogBuilder.CustomizeLog(lb =>
					{
						var customized = lb.WithMinimumLevel(LogLevel.Info);
						if (dbMinimumLevel >= LogLevel.Info)
							customized = customized.SelectEvents(le => le.Level >= dbMinimumLevel || !IsDbSource(le));
						return customized;
					});
				});
			}
		}

		private static bool IsDbSource(LogEvent le)
		{
			if (le.Properties != null
				&& le.Properties.TryGetValue("sourceContext", out var sourceContextValue)
				&& sourceContextValue is SourceContextValue value)
			{
				return value.ToString().Contains("Microsoft.EntityFrameworkCore.Database.Command")
					|| value.ToString().Contains("Microsoft.EntityFrameworkCore.Infrastructure");
			}
			return false;
		}

// Для совместимости с настройками appsettings.json, написанными для серилога
		private static bool TryParseLogLevel(string str, out LogLevel level)
		{
			if (Enum.TryParse(str, true, out level) && Enum.IsDefined(typeof(LogLevel), level))
				return true;
			str = str.ToLowerInvariant();
			switch (str)
			{
				case "verbose":
					level = LogLevel.Debug;
					return true;
				case "debug":
					level = LogLevel.Debug;
					return true;
				case "information":
					level = LogLevel.Info;
					return true;
				case "warning":
					level = LogLevel.Warn;
					return true;
				case "error":
					level = LogLevel.Error;
					return true;
				case "fatal":
					level = LogLevel.Fatal;
					return true;
				default:
					return false;
			}
		}

		private IConfigurationSource GetConfigurationSource()
		{
			var configurationSource = new CommandLineSource(commandLineArguments)
				.CombineWith(new JsonFileSource("appsettings.json"))
				.CombineWith(new JsonFileSource("appsettings." + (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production") + ".json"));
			var environmentName = Environment.GetEnvironmentVariable("UlearnEnvironmentName");
			if (environmentName != null && environmentName.ToLower().Contains("local"))
				configurationSource = configurationSource.CombineWith(new JsonFileSource("appsettings.local.json"));
			return configurationSource;
		}
	}
}