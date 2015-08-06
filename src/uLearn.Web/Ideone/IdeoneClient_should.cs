using System;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using NUnit.Framework;

namespace uLearn.Web.Ideone
{
	[TestFixture]
	public class IdeoneClient_should
	{
		private readonly IdeoneClient service = new IdeoneClient(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30));
		
		[Test, UseReporter(typeof(DiffReporter))]
		[Explicit]
		public void make_submition()
		{
			GetSubmitionDetailsResult res =
				service.Submit(@"using System; public class M{static void Main(){System.Console.WriteLine(42);}}",
					"")
					.Result;
			VerifyResult(res);
		}

		private static void VerifyResult(GetSubmitionDetailsResult res)
		{
			var output = res.WritePropertiesToString().ExcludeLinesWith("Memory:", "Time:", "Date:");
			Console.WriteLine(output);
			Approvals.Verify(output);
		}

		[Test]
		[Explicit]
		public void compile_tuple()
		{
			GetSubmitionDetailsResult res =
				service.Submit(@"using System; public class M{static void Main(){System.Console.WriteLine(Tuple.Create(1, 2));}}",
					"")
					.Result;
			VerifyResult(res);
		}

		[Test]
		[Explicit]
		public void get_supported_languages()
		{
			foreach (var lang in service.GetSupportedLanguages())
			{
				Console.WriteLine(lang);
			}
		}

	}

}