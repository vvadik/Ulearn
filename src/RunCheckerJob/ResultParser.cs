using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Newtonsoft.Json;
using Ulearn.Core.RunCheckerJobApi;

namespace RunCheckerJob
{
	public static class ResultParser
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(ResultParser));

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
				log.Warn("Не удалось распарсить результат");
				return response;
			}
		}
	}
}