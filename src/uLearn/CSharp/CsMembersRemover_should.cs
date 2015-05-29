using System;
using System.IO;
using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;
using uLearn.Model.Blocks;

namespace uLearn.CSharp
{
	[TestFixture]
	public class CsMembersRemover_should
	{
		private readonly FileSystem fs = new FileSystem(new DirectoryInfo("tests"));

		private string LoadCode(string file = "OverloadedMethods.cs")
		{
			return fs.GetContent(file);
		}

		private string RemoveRegion(string region, bool onlyBody = false)
		{
			var code = LoadCode();
			new CsMembersRemover().Remove(ref code, new[] { new Label { Name = region, OnlyBody = onlyBody } });
			return code;
		}

		private string RemoveRegions(params Label[] regions)
		{
			var code = LoadCode();
			new CsMembersRemover().Remove(ref code, regions);
			return code;
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void SingleMethod()
		{
			Approvals.Verify(RemoveRegion("Main"));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void SingleMethodBody()
		{
			Approvals.Verify(RemoveRegion("Main", true));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void OverloadMethod()
		{
			Approvals.Verify(RemoveRegion("Method"));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void OverloadMethodBody()
		{
			Approvals.Verify(RemoveRegion("Method", true));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void SingleClass()
		{
			Approvals.Verify(RemoveRegion("OverloadedMethods"));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void SingleClassBody()
		{
			Approvals.Verify(RemoveRegion("OverloadedMethods", true));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void SingleMethodSolution()
		{
			var code = LoadCode();
			var index = new CsMembersRemover().RemoveSolution(ref code, new Label { Name = "Main" });
			Approvals.Verify(String.Format("solution index: {0}\r\nCode:\r\n{1}", index, code));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void SingleMethodBodySolution()
		{
			var code = LoadCode();
			var index = new CsMembersRemover().RemoveSolution(ref code, new Label { Name = "Main", OnlyBody = true });
			Approvals.Verify(String.Format("solution index: {0}\r\nCode:\r\n{1}", index, code));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void RemoveUsings()
		{
			var code = LoadCode();
			Approvals.Verify(CsMembersRemover.RemoveUsings(code));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void RemoveManyMethods()
		{
			Approvals.Verify(RemoveRegions(new Label { Name = "Method" }, new Label { Name = "Main" }));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void RemoveManyMethodsSameBody()
		{
			Approvals.Verify(RemoveRegions(new Label { Name = "Method", OnlyBody = true }, new Label { Name = "Main" }));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void RemoveManyMethodsAllBody()
		{
			Approvals.Verify(RemoveRegions(new Label { Name = "Method", OnlyBody = true }, new Label { Name = "Main", OnlyBody = true }));
		}
	}
}