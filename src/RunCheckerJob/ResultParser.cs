using System;
using Vostok.Logging.Abstractions;
using Newtonsoft.Json;
using Ulearn.Core.RunCheckerJobApi;

namespace RunCheckerJob
{
	public static class ResultParser
	{
		public static RunningResults Parse(RunningResults response)
		{
			try
			{
				var result = JsonConvert.DeserializeObject<RunningResults>(response.Output);
				if (result == null)
					throw new Exception();
				return result;
			}
			catch (Exception)
			{
				LogProvider.Get().Warn("Не удалось распарсить результат");
				return response;
			}
		}
	}
}