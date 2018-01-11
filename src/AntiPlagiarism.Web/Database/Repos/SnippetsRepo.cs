using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiPlagiarism.Web.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AntiPlagiarism.Web.Database.Repos
{
	public interface ISnippetsRepo
	{
		Task<Snippet> AddSnippetAsync(int tokensCount, SnippetType type, int hash);
		Task<bool> IsSnippetExistsAsync(int tokensCount, SnippetType type, int hash);
		Task<SnippetOccurence> AddSnippetOccurence(int submissionId, int snippetId, int position);
		Task<List<SnippetOccurence>> GetSnippetsOccurencesForSubmission(int submissionId);
	}

	public class SnippetsRepo : ISnippetsRepo
	{
		private readonly AntiPlagiarismDb db;

		public SnippetsRepo(AntiPlagiarismDb db)
		{
			this.db = db;
		}

		public async Task<Snippet> AddSnippetAsync(int tokensCount, SnippetType type, int hash)
		{
			var snippet = new Snippet
			{
				TokensCount = tokensCount,
				SnippetType = type,
				Hash = hash
			};
			await db.Snippets.AddAsync(snippet);
			await db.SaveChangesAsync();
			return snippet;
		}

		public Task<bool> IsSnippetExistsAsync(int tokensCount, SnippetType type, int hash)
		{
			return db.Snippets.AnyAsync(s => s.TokensCount == tokensCount && s.SnippetType == type && s.Hash == hash);
		}

		public async Task<SnippetOccurence> AddSnippetOccurence(int submissionId, int snippetId, int position)
		{
			var snippetOccurence = new SnippetOccurence
			{
				SubmissionId = submissionId,
				SnippetId = snippetId,
				Position = position,
			};
			await db.SnippetsOccurences.AddAsync(snippetOccurence);
			await db.SaveChangesAsync();
			return snippetOccurence;
		}

		public Task<List<SnippetOccurence>> GetSnippetsOccurencesForSubmission(int submissionId)
		{
			return db.SnippetsOccurences.Where(o => o.SubmissionId == submissionId).ToListAsync();
		}

		public async Task<List<Snippet>> GetSnippetsForSubmission(int submissionId)
		{
			return (await GetSnippetsOccurencesForSubmission(submissionId)).Select(o => o.Snippet).ToList();
		}
	}
}