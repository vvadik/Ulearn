namespace UI.Tests.PageObjects.PageObjects
{
	public abstract class SlidePageBlock
	{ }


	public class SlidePageVideoBlock : SlidePageBlock
	{
		
	}

	public class SlidePageTextBlock : SlidePageBlock
	{
		public SlidePageTextBlock(string text)
		{
			Text = text;
		}

		public string Text { get; private set; }
	}

	public class SlidePageCodeBlock : SlidePageTextBlock
	{
		public SlidePageCodeBlock(string text, bool isUserCode)
			: base(text)
		{
			IsUserCode = isUserCode;
		}

		public bool IsUserCode { get; private set; }
	}
}
