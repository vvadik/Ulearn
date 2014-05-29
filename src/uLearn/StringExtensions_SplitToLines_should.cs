using NUnit.Framework;

namespace uLearn
{
	[TestFixture]
	public class StringExtensions_splitToLines_should
	{
		[Test]
		public void not_produce_extra_lines()
		{
			Assert.That("hello\r\nworld".SplitToLines(), Is.EqualTo(new[] {"hello", "world"}).AsCollection);
		}

		[Test]
		public void split_text_with_unix_line_endings()
		{
			Assert.That("hello\nworld".SplitToLines(), Is.EqualTo(new[] {"hello", "world"}).AsCollection);
		}
	}
}