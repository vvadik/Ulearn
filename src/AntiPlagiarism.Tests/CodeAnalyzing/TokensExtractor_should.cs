using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using AntiPlagiarism.Web.CodeAnalyzing;
using NUnit.Framework;
using Serilog;
using Ulearn.Common;
using Ulearn.Common.Extensions;

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

		private List<Token> SkipIfNoPygmentize(Func<List<Token>> getTokens)
		{
			try
			{
				return getTokens();
			}
			catch (Win32Exception ex) when (ex.Message.Contains("The system cannot find the file specified"))
			{
				throw new IgnoreException(ex.Message);
			}
		}

		[Test]
		public void CodeLengthEqualsTokensContentLengthTest()
		{
			const string code = CommonTestData.SimpleProgramWithMethodAndProperty;
			var tokens = SkipIfNoPygmentize(() => tokensExtractor.GetAllTokensFromPygmentize(code, Language.CSharp).EmptyIfNull().ToList());
			Assert.IsNotEmpty(tokens);
			TokensExtractor.ThrowExceptionIfTokensNotMatchOriginalCode(code, tokens);
		}

		[Test]
		public void TokensFiltersTest()
		{
			const string code = CommonTestData.SimpleProgramWithMethodAndProperty;
			var tokens = SkipIfNoPygmentize(() => tokensExtractor.GetFilteredTokensFromPygmentize(code, Language.CSharp));
			Assert.IsNotEmpty(tokens);
			Assert.False(tokens.Any(t => string.IsNullOrWhiteSpace(t.Value)));
			Assert.False(tokens.Any(t => t.Type.StartsWith("Comment")));
		}

		[Test]
		[TestCase("countFlashcardsStatistics.js", Language.JavaScript)]
		[TestCase("geoip.py", Language.Python3)]
		[TestCase("index.html", Language.Html)]
		[TestCase("example.java", Language.Java)]
		[TestCase("example.hs", Language.Haskell)]
		public void LanguagesTest(string file, Language language)
		{
			var code = File.ReadAllText(TestDataDir.GetFile(file).FullName);
			var tokens = SkipIfNoPygmentize(() => tokensExtractor.GetFilteredTokensFromPygmentize(code, language));
			Assert.IsNotEmpty(tokens);
			Assert.False(tokens.Any(t => t.Type.Contains("error")));
		}
	}
}