using System.Threading.Tasks;
using log4net.Config;
using NUnit.Framework;

namespace Stepik.Api.Tests
{
	[TestFixture]
	public class YoutubeVideoUrlExtractorTests
	{
		private YoutubeVideoUrlExtractor extractor;

		[SetUp]
		public void SetUp()
		{
			BasicConfigurator.Configure();
			extractor = new YoutubeVideoUrlExtractor();
		}

		[Test]
		[TestCase("https://www.youtube.com/watch?v=ZSoifyky1Vo")]
		public async Task TestGetVideoUrl(string youtubeVideoUrl)
		{
			var videoUrl = await extractor.GetVideoUrl(youtubeVideoUrl);
			Assert.IsFalse(string.IsNullOrEmpty(videoUrl));
		}
	}
}
