using Serilog.Events;

namespace Ulearn.Common.Api.Helpers
{
	public static class LogEventHelpers
	{
		public static bool IsDbSource(LogEvent le)
		{
			if (le.Properties.TryGetValue("SourceContext", out var sourceContextValue)
					&& (sourceContextValue as ScalarValue)?.Value is string sourceContext)
				return sourceContext.StartsWith("\"Microsoft.EntityFrameworkCore.Database.Command")
					|| sourceContext.StartsWith("\"Microsoft.EntityFrameworkCore.Infrastructure");
			return false;
		}
	}
}