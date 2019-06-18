using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Newtonsoft.Json;
using Ulearn.Core.RunCheckerJobApi;

namespace RunCheckerJob.Js
{
	public class JsResultParser : IResultParser
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(JsResultParser));

		public RunningResults Parse(string json)
		{
			try
			{
				var result = JsonConvert.DeserializeObject<JsTestResult>(json);
				return MakeVerdict(result);
			}
			catch (Exception)
			{
				log.Info("Не удалось распарсить результат тестов");
				throw;
			}
		}

		private static RunningResults MakeVerdict(JsTestResult result)
		{
			var hasUnitTests = result.unit.stats != null;
			if (hasUnitTests && result.unit.stats.failures != 0)
			{
				var failure = result.unit.failures.First();
				return new RunningResults(Verdict.RuntimeError, error: $"{failure.fullTitle}: {failure.err.message}");
			}
			
			var hasUiTests = result.ui.stats != null;
			if (hasUiTests && result.ui.stats.failures != 0)
			{
				var failure = result.ui.failures.First();
				return new RunningResults(Verdict.RuntimeError, error: $"{failure.fullTitle}: {failure.err.message}");
			}

			return new RunningResults(Verdict.Ok);
		}

#pragma warning disable 649
// ReSharper disable ClassNeverInstantiated.Local, CollectionNeverUpdated.Local
private class JsTestResult
		{
			public MochaResult ui;
			public MochaResult unit;
		}

		private class MochaResult
		{
			public List<MochaTest> failures;
			public List<MochaTest> passes;
			public List<MochaTest> pending;
			public MochaStats stats;
			public List<MochaTest> tests;
		}

		private class MochaTest
		{
			public int currentRetry;
			public int duration;
			public JsError err;
			public string fullTitle;
			public string title;
		}

		private class JsError
		{
			public string message;
			public string stack;
		}

		private class MochaStats
		{
			public int duration;
			public DateTime end;
			public int failures;
			public int passes;
			public int pending;
			public DateTime start;
			public int suites;
			public int tests;
		}
	}
}