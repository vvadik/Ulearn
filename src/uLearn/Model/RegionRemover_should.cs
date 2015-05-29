using System.IO;
using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;
using uLearn.Model.Blocks;

namespace uLearn.Model
{
	[TestFixture]
	public class RegionRemover_should
	{
		private static readonly DirectoryInfo dir = new DirectoryInfo("tests");

		private static string LoadCode(string file = "OverloadedMethods.cs")
		{
			return dir.GetFile(file).ContentAsUtf8();
		}

		private static string RemoveRegion(string region, bool onlyBody = false, string langId = "cs")
		{
			var code = LoadCode();
			new RegionRemover(langId).Remove(ref code, new[] { new Label { Name = region, OnlyBody = onlyBody } });
			return code;
		}

		private static string RemoveRegions(string langId, params Label[] regions)
		{
			var code = LoadCode();
			new RegionRemover(langId).Remove(ref code, regions);
			return code;
		}


		[Test, UseReporter(typeof(DiffReporter))]
		public void RemoveMethods()
		{
			Approvals.Verify(RemoveRegion("Method"));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void RemoveRegion()
		{
			Approvals.Verify(RemoveRegion("methods_1_and_2"));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void RemoveMix()
		{
			Approvals.Verify(RemoveRegions("cs", new Label { Name = "methods_1_and_2" }, new Label { Name = "Main" }));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void RemoveMethodsBody()
		{
			Approvals.Verify(RemoveRegion("Method", true));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void RemovePriority()
		{
			Approvals.Verify(RemoveRegion("Region"));
		}
	}
}