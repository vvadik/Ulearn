using System.IO;
using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;
using uLearn.CSharp;

namespace uLearn.Lessions
{
	[TestFixture]
	public class CommonSingleRegionExtractor_should
	{
		private readonly FileSystem fs = new FileSystem(new DirectoryInfo("tests"));

		private string GetRegion(string region, string fileName = "OverloadedMethods.cs")
		{
			var code = fs.GetContent(fileName);
			var extractor = new CommonSingleRegionExtractor(code);
			return extractor.GetRegion(new Label { Name = region });
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void ExtractAllFile()
		{
			Approvals.Verify(GetRegion("all"));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void ExtractOverlap1()
		{
			Approvals.Verify(GetRegion("methods_1_and_2"));
		}

		[Test, UseReporter(typeof(DiffReporter))]
		public void ExtractOverlap2()
		{
			Approvals.Verify(GetRegion("methods_2_and_3"));
		}
	}
}