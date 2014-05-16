using System.Linq;
using NUnit.Framework;

namespace SharpLessons
{
	[TestFixture]
	public class ResourceLoader_should
	{
		[Test]
		public void enumerate_in_Content_folder()
		{
			Assert.That(
				ResourceLoader.EnumerateResourcesFrom("Content").Any(r => r.FullName == "SharpLessons.Content.bootstrap.min.css"));
			foreach (var n in ResourceLoader.EnumerateResourcesFrom("Content"))
				ResourceLoader.LoadResource(n.FullName);
		}

		public void load_by_short_path()
		{
			ResourceLoader.LoadResource("templates.coourse-page.cshtml");
		}
	}
}