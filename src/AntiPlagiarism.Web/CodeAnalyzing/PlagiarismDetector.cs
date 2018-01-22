using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiPlagiarism.Api.Models;
using AntiPlagiarism.Api.Models.Results;
using AntiPlagiarism.Web.CodeAnalyzing.CSharp;
using AntiPlagiarism.Web.Database.Models;
using AntiPlagiarism.Web.Database.Repos;
using AntiPlagiarism.Web.Extensions;
using JetBrains.Annotations;
using uLearn;

namespace AntiPlagiarism.Web.CodeAnalyzing
{
	public class PlagiarismDetector
	{
		private readonly ISnippetsRepo snippetsRepo;
		private readonly ISubmissionsRepo submissionsRepo;
		private readonly CodeUnitsExtractor codeUnitsExtractor;

		public PlagiarismDetector(
			ISnippetsRepo snippetsRepo, ISubmissionsRepo submissionsRepo,
			CodeUnitsExtractor codeUnitsExtractor)
		{
			this.snippetsRepo = snippetsRepo;
			this.submissionsRepo = submissionsRepo;
			this.codeUnitsExtractor = codeUnitsExtractor;
		}

		public async Task<double> GetWeightAsync(Submission firstSubmission, Submission secondSubmission)
		{
			var snippetsOccurencesOfFirstSubmission = await snippetsRepo.GetSnippetsOccurencesForSubmissionAsync(firstSubmission.Id);
			var snippetsOccurencesOfSecondSubmission = await snippetsRepo.GetSnippetsOccurencesForSubmissionAsync(secondSubmission.Id);

			var tokensMatchedInFirstSubmission = new DefaultDictionary<SnippetType, HashSet<int>>();
			var tokensMatchedInSecondSubmission = new DefaultDictionary<SnippetType, HashSet<int>>();
			foreach (var snippetOccurence in snippetsOccurencesOfFirstSubmission)
			{
				var snippet = snippetOccurence.Snippet;
				foreach (var otherOccurence in snippetsOccurencesOfSecondSubmission.Where(o => o.SnippetId == snippet.Id))
				{
					for (var i = 0; i < snippet.TokensCount; i++)
					{
						tokensMatchedInFirstSubmission[snippet.SnippetType].Add(snippetOccurence.FirstTokenIndex + i);
						tokensMatchedInSecondSubmission[snippet.SnippetType].Add(otherOccurence.FirstTokenIndex + i);
					}
				}
			}

			var unionLength = 0;
			var allSnippetTypes = GetAllSnippetTypes();
			foreach (var snippetType in allSnippetTypes)
			{
				if (!tokensMatchedInFirstSubmission.ContainsKey(snippetType))
					continue;
					
				unionLength += tokensMatchedInFirstSubmission[snippetType].Count;
				unionLength += tokensMatchedInSecondSubmission[snippetType].Count;
			}
			
			var totalLength = firstSubmission.TokensCount + secondSubmission.TokensCount;
			var weight = ((double)unionLength) / totalLength;
			
			/* Normalize weight */
			weight /= allSnippetTypes.Count;
			return weight;
		}

		public async Task<List<Plagiarism>> GetPlagiarismsAsync(Submission submission)
		{
			/* Dictionaries by submission id and snippet type */
			var tokensMatchedInThisSubmission = new DefaultDictionary<Tuple<int, SnippetType>, HashSet<int>>();
			var tokensMatchedInOtherSubmissions = new DefaultDictionary<Tuple<int, SnippetType>, HashSet<int>>();
			
			var snippetsOccurences = await snippetsRepo.GetSnippetsOccurencesForSubmissionAsync(submission.Id);
			var matchedSnippets = new DefaultDictionary<int, List<MatchedSnippet>>();
			foreach (var snippetOccurence in snippetsOccurences)
			{
				var otherOccurences = await snippetsRepo.GetSnippetsOccurencesAsync(
					snippetOccurence.SnippetId,
					/* Filter only snippet occurences in submissions BY THIS client, THIS task, THIS language and NOT BY THIS author */
					o => o.Submission.ClientId == submission.ClientId &&
						o.Submission.TaskId == submission.TaskId &&
						o.Submission.Language == submission.Language &&
						o.Submission.AuthorId != submission.AuthorId
				);

				var snippetType = snippetOccurence.Snippet.SnippetType;

				foreach (var otherOccurence in otherOccurences)
				{
					for (var i = 0; i < snippetOccurence.Snippet.TokensCount; i++)
					{
						var tokenIndexInThisSubmission = snippetOccurence.FirstTokenIndex + i;
						var tokenIndexInOtherSubmission = otherOccurence.FirstTokenIndex + i;
						tokensMatchedInThisSubmission[Tuple.Create(otherOccurence.SubmissionId, snippetType)].Add(tokenIndexInThisSubmission);
						tokensMatchedInOtherSubmissions[Tuple.Create(otherOccurence.SubmissionId, snippetType)].Add(tokenIndexInOtherSubmission);
					}

					matchedSnippets[otherOccurence.SubmissionId].Add(new MatchedSnippet
					{
						SnippetType = snippetType,
						TokensCount = snippetOccurence.Snippet.TokensCount,
						OriginalSubmissionFirstTokenIndex = snippetOccurence.FirstTokenIndex,
						PlagiarismSubmissionFirstTokenIndex = otherOccurence.FirstTokenIndex,
					});
				}
			}

			var plagiateSubmissionIds = tokensMatchedInOtherSubmissions.Keys.Select(tuple => tuple.Item1).ToList();
			var plagiateSubmissions = await submissionsRepo.GetSubmissionsByIdsAsync(plagiateSubmissionIds);

			var plagiarisms = new List<Plagiarism>();
			
			var allSnippetTypes = GetAllSnippetTypes();
			var thisSubmissionLength = submission.TokensCount;
			foreach (var plagiarismSubmission in plagiateSubmissions)
			{
				var unionLength = 0;
				foreach (var snippetType in allSnippetTypes)
				{
					var submissionIdWithSnippetType = Tuple.Create(plagiarismSubmission.Id, snippetType);
					if (!tokensMatchedInThisSubmission.ContainsKey(submissionIdWithSnippetType))
						continue;
					
					unionLength += tokensMatchedInThisSubmission[submissionIdWithSnippetType].Count;
					unionLength += tokensMatchedInOtherSubmissions[submissionIdWithSnippetType].Count;
				}

				var plagiateSubmissionLength = plagiarismSubmission.TokensCount;
				var totalLength = thisSubmissionLength + plagiateSubmissionLength;
				var weight = ((double)unionLength) / totalLength;
				/* Normalize weight */
				weight /= allSnippetTypes.Count;
				
				plagiarisms.Add(BuildPlagiarismInfo(plagiarismSubmission, weight, matchedSnippets[plagiarismSubmission.Id]));
			}

			return plagiarisms;
		}

		public List<TokenPosition> GetNeededTokensPositions(string program)
		{
			return GetNeededTokensPositions(codeUnitsExtractor.Extract(program));
		}

		public static List<TokenPosition> GetNeededTokensPositions(IEnumerable<CodeUnit> codeUnits)
		{
			return codeUnits.SelectMany(u => u.Tokens.Enumerate(start: u.FirstTokenIndex)).Select(t => new TokenPosition
			{
				TokenIndex = t.Index,
				StartPosition = t.Item.SpanStart,
				Length = t.Item.Span.Length,
			}).ToList();
		}

		private Plagiarism BuildPlagiarismInfo(Submission submission, double weight, List<MatchedSnippet> matchedSnippets)
		{
			var codeUnits = codeUnitsExtractor.Extract(submission.ProgramText);
			return new Plagiarism
			{
				Submission = new PlagiarismSubmission
				{
					Id = submission.Id,
					TaskId = submission.TaskId,
					AuthorId = submission.AuthorId,
					Code = submission.ProgramText,
					AdditionalInfo = submission.AdditionalInfo,
				},
				Weight = weight,
				AnalyzedCodeUnits = codeUnits.Select(
					u => new AnalyzedCodeUnit
					{
						Name = u.Path.ToString(),
						FirstTokenIndex = u.FirstTokenIndex,
						TokensCount = u.Tokens.Count,
					}).ToList(),
				TokensPositions = GetNeededTokensPositions(codeUnits),
				MatchedSnippets = matchedSnippets, 
			};
		}

		private static List<SnippetType> GetAllSnippetTypes()
		{
			return Enum.GetValues(typeof(SnippetType)).Cast<SnippetType>().ToList();
		}
	}
}