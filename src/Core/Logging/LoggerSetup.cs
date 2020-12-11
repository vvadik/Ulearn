using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions;
using Ulearn.Core.Configuration;
using Vostok.Logging.Abstractions.Values;
using Vostok.Logging.Console;
using Vostok.Logging.File;
using Vostok.Logging.File.Configuration;
using Vostok.Logging.Formatting;
using LogEvent = Vostok.Logging.Abstractions.LogEvent;

namespace Ulearn.Core.Logging
{
	public static class LoggerSetup
	{
		public static readonly OutputTemplate OutputTemplate
			= OutputTemplate.Parse("{Timestamp:HH:mm:ss.fff} {Level:u5} {traceContext:w}{operationContext:w}{sourceContext:w}{threadId:w}{address:w}{user:w} {Message}{NewLine}{Exception}");

		public static void SetupForTests()
		{
			var log = new ConsoleLog(new ConsoleLogSettings { OutputTemplate = OutputTemplate }).WithMinimumLevel(LogLevel.Debug);
			LogProvider.Configure(log, true);
		}

		public static ILog Setup(HostLogConfiguration hostLog, string subdirectory = null, bool addInLogProvider = true)
		{
			ILog consoleLog = null;
			if (hostLog.Console)
				consoleLog = new ConsoleLog(new ConsoleLogSettings { OutputTemplate = OutputTemplate })
					.WithMinimumLevel(LogLevel.Info);

			ILog telegramLog = null;
			if (hostLog.ErrorLogsToTelegram)
				telegramLog = new TelegramLog { OutputTemplate = OutputTemplate }
					.WithMinimumLevel(LogLevel.Error);

			var pathFormat = hostLog.PathFormat;
			ILog fileLog = null;
			if (!string.IsNullOrEmpty(pathFormat))
			{
				pathFormat = pathFormat.Replace("{Date}", "{RollingSuffix}"); // Для совместимости с настройками appsettings.json, написанными для серилога
				if (Path.IsPathRooted(pathFormat) && subdirectory != null)
				{
					var directory = Path.GetDirectoryName(pathFormat);
					var fileName = Path.GetFileName(pathFormat);
					pathFormat = Path.Combine(directory, subdirectory, fileName);
				}

				var minimumLevelString = hostLog.MinimumLevel ?? "debug";
				var dbMinimumLevelString = hostLog.DbMinimumLevel ?? "";
				if (!TryParseLogLevel(minimumLevelString, out var minimumLevel))
					minimumLevel = LogLevel.Debug;
				if (!TryParseLogLevel(dbMinimumLevelString, out var dbMinimumLevel))
					dbMinimumLevel = minimumLevel;

				var fileLogSettings = new FileLogSettings
				{
					FilePath = pathFormat,
					RollingStrategy = new RollingStrategyOptions
					{
						MaxFiles = 0,
						Type = RollingStrategyType.Hybrid,
						Period = RollingPeriod.Day,
						MaxSize = 4 * 1073741824L,
					},
					OutputTemplate = OutputTemplate
				};
				var min = dbMinimumLevel > minimumLevel ? minimumLevel : dbMinimumLevel;
				fileLog = new FileLog(fileLogSettings)
					.DropEvents(FilterLogs)
					.SelectEvents(le => le.Level >= (IsDbSource(le) ? dbMinimumLevel : minimumLevel))
					.WithMinimumLevel(min);
			}

			var log = new CompositeLog(new[] { fileLog, consoleLog, telegramLog }.Where(l => l != null).ToArray())
				.WithMinimumLevel(LogLevel.Debug);

			if (addInLogProvider)
				LogProvider.Configure(log);
			return log;
		}

		[CanBeNull]
		public static SourceContextValue GetSourceContext([CanBeNull] LogEvent @event)
		{
			if (@event?.Properties == null)
				return null;
			return @event.Properties.TryGetValue(WellKnownProperties.SourceContext, out var value) ? value as SourceContextValue : null;
		}

		public static bool IsDbSource(LogEvent le)
		{
			var sourceContext = GetSourceContext(le);
			if (sourceContext == null)
				return false;
			return sourceContext.Any(str =>
				str.StartsWith("Microsoft.EntityFrameworkCore.Database.Command")
				|| str.StartsWith("Microsoft.EntityFrameworkCore.Infrastructure")
			);
		}

		public static bool FilterLogs(LogEvent le)
		{
			var sourceContext = GetSourceContext(le);
			if (sourceContext == null)
				return false;
			return sourceContext.Any(str =>
				str.StartsWith("Microsoft.AspNetCore.Mvc.Infrastructure")
				|| str.StartsWith("Microsoft.AspNetCore.Hosting.Diagnostics")
				|| str.StartsWith("Microsoft.AspNetCore.Cors.Infrastructure")
				|| str.StartsWith("Microsoft.AspNetCore.Authentication")
				|| str.StartsWith("Microsoft.AspNetCore.Authorization")
			);
		}

		// Для совместимости с настройками appsettings.json, написанными для серилога
		public static bool TryParseLogLevel(string str, out LogLevel level)
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
	}
}