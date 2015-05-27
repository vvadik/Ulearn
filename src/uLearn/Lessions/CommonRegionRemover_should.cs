using System.IO;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;

namespace uLearn.Lessions
{
	[TestFixture]
	public class CommonRegionRemover_should
	{
		private static readonly DirectoryInfo dir = new DirectoryInfo("tests");

		private static string RemoveLabels(string file, params string[] labels)
		{
			var code = dir.GetFile(file).ContentAsUtf8();
			var remover = new CommonRegionRemover();
			Assert.IsEmpty(remover.Remove(ref code, labels.Select(s => new Label { Name = s })));
			return code;
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public static void RemoveRegion()
		{
			Approvals.Verify(RemoveLabels("OverloadedMethods.cs", "methods_1_and_2"));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public static void RemoveEmbeddedRegions()
		{
			Approvals.Verify(RemoveLabels("regions.txt", "b", "c"));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public static void RemoveOverlapedRegions()
		{
			Approvals.Verify(RemoveLabels("regions.txt", "d", "e"));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public static void RemoveRegions()
		{
			Approvals.Verify(RemoveLabels("regions.txt", "f", "g", "h"));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public static void RemoveRegionsCode()
		{
			Approvals.Verify(RemoveLabels("OverloadedMethods.cs", "methods_1_and_2", "methods_2_and_3"));
		}
	}
}