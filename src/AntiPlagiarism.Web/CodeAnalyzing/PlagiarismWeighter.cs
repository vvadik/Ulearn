using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiPlagiarism.Web.Database.Models;
using AntiPlagiarism.Web.Database.Repos;
using uLearn;

namespace AntiPlagiarism.Web.CodeAnalyzing
{
	public class PlagiarismWeighter
	{
		private readonly ISnippetsRepo snippetsRepo;

		public PlagiarismWeighter(ISnippetsRepo snippetsRepo)
		{
			this.snippetsRepo = snippetsRepo;
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

			var totalUnion = 0;
			var allSnippetTypes = Enum.GetValues(typeof(SnippetType)).Cast<SnippetType>().ToList();
			foreach (var snippetType in allSnippetTypes)
			{
				if (!tokensMatchedInFirstSubmission.ContainsKey(snippetType))
					continue;
					
				totalUnion += tokensMatchedInFirstSubmission[snippetType].Count;
				totalUnion += tokensMatchedInSecondSubmission[snippetType].Count;
			}
			
			var totalLength = firstSubmission.TokensCount + secondSubmission.TokensCount;
			var weight = ((double)totalUnion) / totalLength;
			
			/* Normalize weight */
			weight /= allSnippetTypes.Count;
			return weight;
		}
	}
}