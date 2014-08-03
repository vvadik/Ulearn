using System;
using System.IO;
using NUnit.Framework;

namespace uLearn.Web.Ideone
{
	[TestFixture]
	public class ExecutionService_should
	{
		private readonly ExecutionService service = new ExecutionService();

		[Test]
		[Explicit]
		public void make_submition()
		{
			GetSubmitionDetailsResult res =
				service.Submit(@"using System; public class M{static void Main(){System.Console.WriteLine(42);}}",
					"")
					.Result;
			Console.WriteLine(res.Error);
			Console.WriteLine(res.Status);
			Console.WriteLine(res.Result);
			Console.WriteLine(res.LangId);
			Console.WriteLine(res.LangName);
			Console.WriteLine(res.LangVersion);
			Console.WriteLine(res.IsPublic);
			Console.WriteLine(res.Date);
			Console.WriteLine(res.Source);
			Console.WriteLine(res.CompilationError);
			Console.WriteLine(res.Input);
			Console.WriteLine(res.Output);
			Console.WriteLine(res.StdErr);
			Console.WriteLine(res.Signal);
			Console.WriteLine(res.Time);
			Console.WriteLine(res.Memory);
		}
		
		[Test]
		[Explicit]
		public void compile_tuple()
		{
			GetSubmitionDetailsResult res =
				service.Submit(@"using System; public class M{static void Main(){System.Console.WriteLine(Tuple.Create(1, 2));}}",
					"")
					.Result;
			Console.WriteLine(res.Error);
			Console.WriteLine(res.Status);
			Console.WriteLine(res.Result);
			Console.WriteLine(res.LangId);
			Console.WriteLine(res.LangName);
			Console.WriteLine(res.LangVersion);
			Console.WriteLine(res.IsPublic);
			Console.WriteLine(res.Date);
			Console.WriteLine(res.Source);
			Console.WriteLine(res.CompilationError);
			Console.WriteLine(res.Input);
			Console.WriteLine(res.Output);
			Console.WriteLine(res.StdErr);
			Console.WriteLine(res.Signal);
			Console.WriteLine(res.Time);
			Console.WriteLine(res.Memory);
			Assert.AreEqual(SubmitionResult.Success, res.Result);
		}

		[Test]
		public void get_supported_languages()
		{
			foreach (var lang in service.GetSupportedLanguages())
			{
				Console.WriteLine(lang);
			}
		}

	}

}