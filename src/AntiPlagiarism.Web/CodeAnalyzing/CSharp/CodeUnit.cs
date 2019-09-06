using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace AntiPlagiarism.Web.CodeAnalyzing.CSharp
{
	public class CodeUnit
	{
		public CodePath Path { get; private set; }
		public List<SyntaxToken> Tokens { get; private set; }
		public int FirstTokenIndex { get; set; }

		public int Position => Tokens.FirstOrDefault().SpanStart;

		public CodeUnit(CodePath path, IEnumerable<SyntaxToken> tokens, int firstTokenIndex = 0)
		{
			Path = path;
			Tokens = tokens.ToList();
			FirstTokenIndex = firstTokenIndex;
		}

		public override string ToString()
		{
			var tokensString = string.Join(" ", Tokens.Select(t => t.ToString()));
			return $"CodeUnit({tokensString} at {Path}, position {Position})";
		}
	}
}