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

		public static RunningResults Parse(string output, string error)
		{
			try
			{
				var result = JsonConvert.DeserializeObject<RunningResults>(output);
				return result;
			}
			catch (Exception)
			{
				log.Info("Не удалось распарсить результат");
				return new RunningResults(Verdict.UndefinedError, error: error, output: output);
			}
		}
	}
}