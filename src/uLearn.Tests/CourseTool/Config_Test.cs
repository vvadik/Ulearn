using System.IO;
using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;
using Ulearn.Common.Extensions;

namespace uLearn.CourseTool
{
	[TestFixture]
	public class Config_Test
	{
		[Test]
		[UseReporter(typeof(DiffReporter))]
		public void ConfigTemplate_IsOk()
		{
			var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "templates", "config.xml");
			var config = new FileInfo(path).DeserializeXml<Config>();
			config.IgnoredUlearnSlides = new[] { "1", "2" };
			Approvals.VerifyXml(config.XmlSerialize());
		}
	}
}