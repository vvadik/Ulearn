using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Filters;

namespace uLearn.NUnitTestRunning
{
	/// <summary>
	/// Этот файл включен в ресурсы и добавляется в пришедший на проверку проект с задачей, если у нее в качестве способа проверки 
	/// установлен nunit-test-class, а не startup-object
	/// </summary>
	public class NUnitTestRunner
	{
		public static void Main()
		{
			string[] testClassesToLaunch = { "SHOULD_BE_REPLACED" };
			var listener = new TestListener();
			RunAllTests(listener, Assembly.GetExecutingAssembly(), testClassesToLaunch);
		}

		public static void RunAllTests(ITestListener testListener, Assembly executingAssembly, params string[] testClassesToLaunch)
		{
			var runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
			runner.Load(executingAssembly, new Dictionary<string, object> { { "StopOnError", true } });

			foreach (var testClass in testClassesToLaunch)
			{
				RunTestBatch(testClass, runner, testListener);
				if (runner.Result.ResultState.Status == TestStatus.Failed)
					break;
			}
		}

		private static void RunTestBatch(string testClass, NUnitTestAssemblyRunner runner, ITestListener listener)
		{
			var testFilter = new ClassNameFilter(testClass);
			runner.Run(listener, testFilter);
		}

		// должен остаться в этом же файле
		public class TestListener : ITestListener
		{
			public IReadOnlyCollection<ITestResult> Results => results;

			private readonly List<ITestResult> results = new List<ITestResult>();

			public void TestStarted(ITest test)
			{
			}

			public void TestFinished(ITestResult result)
			{
				results.Add(result);

				if (result.ResultState.Status == TestStatus.Failed && result.ResultState.Site != FailureSite.Child
					&& result.ResultState.Label != "Cancelled")
				{
					Console.WriteLine($"Error on NUnit test: {result.Name} {result.Message} {result.StackTrace}");
				}
			}

			public void TestOutput(TestOutput output)
			{
			}
		}
	}
}
