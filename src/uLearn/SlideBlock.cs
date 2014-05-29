namespace uLearn
{
	public class SlideBlock
	{
		private SlideBlock(string text, bool isCodeSample)
		{
			IsCodeSample = isCodeSample;
			Text = text.TrimEnd();
		}

		public static SlideBlock FromCode(string code)
		{
			return new SlideBlock(code, true);
		}

		public static SlideBlock FromMarkdown(string markdown)
		{
			return new SlideBlock(markdown, false);
		}

		protected bool Equals(SlideBlock other)
		{
			return IsCodeSample.Equals(other.IsCodeSample) && string.Equals(Text, other.Text);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((SlideBlock) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (IsCodeSample.GetHashCode()*397) ^ (Text != null ? Text.GetHashCode() : 0);
			}
		}

		public override string ToString()
		{
			return string.Format("IsCodeSample: {0}, Text: {1}", IsCodeSample, Text);
		}

		public readonly bool IsCodeSample;
		public readonly string Text;

		public string RenderedText
		{
			get { return IsCodeSample ? Text : Md.ToHtml(Text); }
		}

		public SlideBlock WithAppendedText(string text)
		{
			return new SlideBlock(Text + "\n" + text, IsCodeSample);
		}
	}
}