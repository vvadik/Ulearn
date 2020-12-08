using System;
using System.IO;
using Serilog;
using Serilog.Events;
using Ulearn.Core.Configuration;

namespace Ulearn.Core.Logging
{
	public static class LoggerSetup
	{
		private static string OutputTemplate = "{Timestamp:HH:mm:ss.fff} {Level} [{Thread}] {Message}{NewLine}{Exception}";

		public static void SetupForTests()
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.NUnitOutput()
				.WriteTo.Console(outputTemplate: OutputTemplate)
				.CreateLogger();
		}

		public static void SetupForLog4Net()
		{
			Log.Logger = new LoggerConfiguration().WriteTo.Log4Net().CreateLogger();
		}

		public static void SetupWithoutVostok(HostLogConfiguration hostLog, string serviceName)
		{
			var loggerConfiguration = new LoggerConfiguration()
				.MinimumLevel.Debug();

			if (hostLog.Console)
				loggerConfiguration = loggerConfiguration.WriteTo.Console(
					outputTemplate: OutputTemplate,
					restrictedToMinimumLevel: LogEventLevel.Information
				);

			var pathFormat = hostLog.PathFormat;
			if (!Enum.TryParse<LogEventLevel>(hostLog.MinimumLevel, true, out var minimumLevel))
				minimumLevel = LogEventLevel.Debug;
			if (!Enum.TryParse<LogEventLevel>(hostLog.DbMinimumLevel, out var dbMinimumLevel))
				dbMinimumLevel = minimumLevel;

			if (Path.IsPathRooted(pathFormat))
			{
				var directory = Path.GetDirectoryName(pathFormat);
				var fileName = Path.GetFileName(pathFormat);
				pathFormat = Path.Combine(directory, serviceName, fileName);
			}

			loggerConfiguration = loggerConfiguration
				//.Filter.ByExcluding(le => le.Level >= dbMinimumLevel || !IsDbSource(le))
				.WriteTo.RollingFile(
					pathFormat,
					outputTemplate: OutputTemplate,
					restrictedToMinimumLevel: minimumLevel,
					fileSizeLimitBytes: 4 * 1073741824L
				);

			Log.Logger = loggerConfiguration.CreateLogger();
		}


		/*
		//TODO потестировать
		private static bool IsDbSource(LogEvent le)
		{
			if (le.Properties != null
				&& le.Properties.TryGetValue("sourceContext", out var sourceContextValue)
				&& (sourceContextValue as ScalarValue)?.Value is string sourceContext)
			{
				return sourceContext.StartsWith("Microsoft.EntityFrameworkCore.Database.Command")
					|| sourceContext.StartsWith("Microsoft.EntityFrameworkCore.Infrastructure");
			}
			return false;
		}
		*/
	}
}