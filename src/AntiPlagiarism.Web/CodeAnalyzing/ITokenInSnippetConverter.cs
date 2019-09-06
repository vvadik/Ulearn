using AntiPlagiarism.Api.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AntiPlagiarism.Web.CodeAnalyzing
{
	public interface ITokenInSnippetConverter
	{
		string Convert(SyntaxToken token);
		SnippetType SnippetType { get; }
	}

	public class TokensKindsOnlyConverter : ITokenInSnippetConverter
	{
		public string Convert(SyntaxToken token)
		{
			return token.Kind().ToString();
		}

		public SnippetType SnippetType => SnippetType.TokensKindsOnly;
	}

	public class TokensKindsAndValuesConverter : ITokenInSnippetConverter
	{
		public string Convert(SyntaxToken token)
		{
			return token.ToString();
		}

		public SnippetType SnippetType => SnippetType.TokensKindsAndValues;
	}
}