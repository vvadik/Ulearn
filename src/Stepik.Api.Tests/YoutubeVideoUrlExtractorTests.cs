using System.Threading.Tasks;
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