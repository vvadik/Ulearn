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
				if(result == null)
					throw new Exception();
				return result;
			}
			catch (Exception)
			{
				log.Info("Не удалось распарсить результат");
				return new RunningResults(Verdict.RuntimeError, error: error, output: output);
			}
		}
	}
}