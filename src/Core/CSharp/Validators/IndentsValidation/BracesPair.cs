using Microsoft.CodeAnalysis;

namespace uLearn.CSharp.Validators.IndentsValidation
{
	internal class BracesPair
	{
		public readonly SyntaxToken Open;
		public readonly SyntaxToken Close;

		public BracesPair(SyntaxToken open, SyntaxToken close)
		{
			Open = open;
			Close = close;
		}

		public bool TokenInsideBraces(SyntaxToken token)
		{
			return token.SpanStart > Open.SpanStart && token.Span.End < Close.Span.End;
		}

		public override string ToString()
		{
			return $"строки {IndentsSyntaxExtensions.GetLine(Open) + 1}, {IndentsSyntaxExtensions.GetLine(Close) + 1}";
		}
	}
}