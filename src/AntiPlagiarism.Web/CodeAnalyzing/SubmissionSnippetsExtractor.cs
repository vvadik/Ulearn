using System;
using System.Collections.Generic;
using AntiPlagiarism.Web.Configuration;
using AntiPlagiarism.Web.Database.Models;
using Microsoft.Extensions.Options;
using Serilog;
using Ulearn.Common.Extensions;

namespace AntiPlagiarism.Web.CodeAnalyzing
{
	public class SubmissionSnippetsExtractor
	{
		private readonly CodeUnitsExtractor codeUnitsExtractor;
		private readonly SnippetsExtractor snippetsExtractor;
		private readonly ILogger logger = Log.Logger;
		private readonly AntiPlagiarismConfiguration configuration;

		private readonly List<ITokenInSnippetConverter> tokenConverters = new List<ITokenInSnippetConverter>
		{
			new TokensKindsConverter(),
			new TokensValuesConverter(),
		};

		public SubmissionSnippetsExtractor(CodeUnitsExtractor codeUnitsExtractor, SnippetsExtractor snippetsExtractor,
			IOptions<AntiPlagiarismConfiguration> options)
		{
			this.codeUnitsExtractor = codeUnitsExtractor;
			this.snippetsExtractor = snippetsExtractor;
			configuration = options.Value;
		}

		public IEnumerable<Tuple<int, Snippet>> ExtractSnippetsFromSubmission(Submission submission)
		{
			logger.Information("Достаю сниппеты из решения {submissionId}, длина сниппетов: {tokensCount} токенов", submission.Id, configuration.AntiPlagiarism.SnippetTokensCount);
			var codeUnits = codeUnitsExtractor.Extract(submission.ProgramText, submission.Language);
			foreach (var codeUnit in codeUnits)
			{
				foreach (var tokenConverter in tokenConverters)
				{
					var snippets = snippetsExtractor.GetSnippets(codeUnit.Tokens, configuration.AntiPlagiarism.SnippetTokensCount, tokenConverter);
					foreach (var (index, snippet) in snippets.Enumerate())
					{
						yield return Tuple.Create(codeUnit.FirstTokenIndex + index, snippet);
					}
				}
			}
		}
	}
}