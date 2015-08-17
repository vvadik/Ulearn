using System.IO;
using ApprovalTests;
using NUnit.Framework;

namespace uLearn.CourseTool
{
	[TestFixture]
	public class Config_Test
	{
		[Test]
		public void ConfigTemplate_IsOk()
		{
			var config = new FileInfo("templates\\config.xml").DeserializeXml<Config>();
			config.IgnoredSlides = new[] { "1", "2" };
			Approvals.VerifyXml(config.XmlSerialize());
		}
	}
}