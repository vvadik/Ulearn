using System;
using Npgsql.Logging;
using Ulearn.Core.Configuration;
using Ulearn.Core.Logging;
using Vostok.Logging.Abstractions;

namespace Database
{
	internal class UlearnDbLoggingProvider : INpgsqlLoggingProvider
	{
		public NpgsqlLogger CreateLogger(string name)
		{
			return new UlearnDbLogger(name);
		}
	}

	internal class UlearnDbLogger : NpgsqlLogger
	{
		private readonly LogLevel dbMinimumLevel;

		public UlearnDbLogger(string name)
		{
			dbMinimumLevel = GetDbMinimumLevelFromConfig();
		}

		private static LogLevel GetDbMinimumLevelFromConfig()
		{
			var configuration = ApplicationConfiguration.Read<UlearnConfiguration>();
			var minimumLevelString = configuration.HostLog.MinimumLevel ?? "debug";
			var dbMinimumLevelString = configuration.HostLog.DbMinimumLevel ?? "";
			if (!LoggerSetup.TryParseLogLevel(minimumLevelString, out var minimumLevel))
				minimumLevel = LogLevel.Debug;
			if (!LoggerSetup.TryParseLogLevel(dbMinimumLevelString, out var dbMinimumLevel))
				dbMinimumLevel = minimumLevel;
			return dbMinimumLevel;
		}

		public override bool IsEnabled(NpgsqlLogLevel level)
		{
			return ToVostokLogLevel(level) >= dbMinimumLevel;
		}

		public override void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception? exception = null)
		{
			var log = LogProvider.Get().ForContext(nameof(UlearnDb));
			log.Log(new LogEvent(ToVostokLogLevel(level), DateTimeOffset.Now, msg, exception));
		}

		private static LogLevel ToVostokLogLevel(NpgsqlLogLevel level)
		{
			return level switch
			{
				NpgsqlLogLevel.Trace => LogLevel.Debug,
				NpgsqlLogLevel.Debug => LogLevel.Debug,
				NpgsqlLogLevel.Info => LogLevel.Info,
				NpgsqlLogLevel.Warn => LogLevel.Warn,
				NpgsqlLogLevel.Error => LogLevel.Error,
				NpgsqlLogLevel.Fatal => LogLevel.Fatal,
				_ => throw new ArgumentOutOfRangeException(nameof(level))
			};
		}
	}
}