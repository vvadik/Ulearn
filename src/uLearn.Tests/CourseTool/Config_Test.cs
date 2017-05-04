using System.IO;
using ApprovalTests;
using NUnit.Framework;
using uLearn.Extensions;

namespace uLearn.CourseTool
{
	[TestFixture]
	public class Config_Test
	{
		[Test]
		public void ConfigTemplate_IsOk()
		{
			var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "templates", "config.xml");
			var config = new FileInfo(path).DeserializeXml<Config>();
			config.IgnoredUlearnSlides = new[] { "1", "2" };
			Approvals.VerifyXml(config.XmlSerialize());
		}
	}
}