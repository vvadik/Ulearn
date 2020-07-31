using System;
using System.IO;
using System.Linq;
using AntiPlagiarism.Web.CodeAnalyzing;
using NUnit.Framework;
using Serilog;
using Ulearn.Common.Extensions;
using Ulearn.Core;

namespace AntiPlagiarism.Tests.CodeAnalyzing
{
	[TestFixture]
	public class TokensExtractor_should
	{
		private TokensExtractor tokensExtractor;
		
		private static DirectoryInfo TestDataDir => new DirectoryInfo(
			Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..",
				"CodeAnalyzing", "TestData")
		);

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			var logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
			tokensExtractor = new TokensExtractor(logger);
		}

		[Test]
		public void CodeLengthEqualsTokensContentLengthTest()
		{
			const string code = CommonTestData.SimpleProgramWithMethodAndProperty;
			var tokens = tokensExtractor.GetAllTokensFromPygmentize(code, Language.CSharp).EmptyIfNull().ToList();
			TokensExtractor.ThrowExceptionIfTokensNotMatchOriginalCode(code, tokens);
		}

		[Test]
		public void TokensFiltersTest()
		{
			const string code = CommonTestData.SimpleProgramWithMethodAndProperty;
			var tokens = tokensExtractor.GetAllTokensFromPygmentize(code, Language.CSharp).EmptyIfNull().ToList();
			var filteredTokens = TokensExtractor.FilterCommentTokens(TokensExtractor.FilterWhitespaceTokens(tokens)).ToList();
			Assert.False(filteredTokens.Any(t => string.IsNullOrWhiteSpace(t.Value)));
			Assert.False(filteredTokens.Any(t => t.Type.StartsWith("Comment")));
		}

		[Test]
		[TestCase("countFlashcardsStatistics.js", Language.JavaScript)]
		[TestCase("geoip.py", Language.Python3)]
		[TestCase("index.html", Language.Html)]
		public void LanguagesTest(string file, Language language)
		{
			var code = File.ReadAllText(TestDataDir.GetFile(file).FullName);
			var tokens = tokensExtractor.GetAllTokensFromPygmentize(code, language).EmptyIfNull().ToList();
			var filteredTokens = TokensExtractor.FilterCommentTokens(TokensExtractor.FilterWhitespaceTokens(tokens)).ToList();
			Assert.IsNotEmpty(filteredTokens);
			Assert.False(filteredTokens.Any(t => t.Type.Contains("error")));
		}
	}
}