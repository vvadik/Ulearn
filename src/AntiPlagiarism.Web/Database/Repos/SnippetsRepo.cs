using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AntiPlagiarism.Web.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AntiPlagiarism.Web.Database.Repos
{
	public interface ISnippetsRepo
	{
		Task<Snippet> AddSnippetAsync(int tokensCount, SnippetType type, int hash);
		Task<bool> IsSnippetExistsAsync(int tokensCount, SnippetType type, int hash);
		Task<SnippetOccurence> AddSnippetOccurenceAsync(int submissionId, Snippet snippet, int firstTokenIndex);
		Task<List<SnippetOccurence>> GetSnippetsOccurencesForSubmissionAsync(int submissionId);
		Task<List<SnippetOccurence>> GetSnippetsOccurencesAsync(int snippetId);
		Task<List<SnippetOccurence>> GetSnippetsOccurencesAsync(int snippetId, Expression<Func<SnippetOccurence, bool>> filterFunction);
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

		public async Task<SnippetOccurence> AddSnippetOccurenceAsync(int submissionId, Snippet snippet, int firstTokenIndex)
		{
			var foundSnippet = await GetOrAddSnippetAsync(snippet);
			var snippetOccurence = new SnippetOccurence
			{
				SubmissionId = submissionId,
				Snippet = foundSnippet,
				FirstTokenIndex = firstTokenIndex,
			};
			await db.SnippetsOccurences.AddAsync(snippetOccurence);
			await db.SaveChangesAsync();
			return snippetOccurence;
		}

		private async Task<Snippet> GetOrAddSnippetAsync(Snippet snippet)
		{
			using (var transaction = db.Database.BeginTransaction())
			{
				var foundSnippet = await db.Snippets.SingleOrDefaultAsync(
					s => s.SnippetType == snippet.SnippetType
						&& s.TokensCount == snippet.TokensCount
						&& s.Hash == snippet.Hash
				);
				if (foundSnippet != null)
					return foundSnippet;

				await db.Snippets.AddAsync(snippet);
				await db.SaveChangesAsync();
				transaction.Commit();
			}

			return snippet;
		}

		public Task<List<SnippetOccurence>> GetSnippetsOccurencesForSubmissionAsync(int submissionId)
		{
			return db.SnippetsOccurences.Include(o => o.Snippet).Where(o => o.SubmissionId == submissionId).ToListAsync();
		}

		public Task<List<SnippetOccurence>> GetSnippetsOccurencesAsync(int snippetId)
		{
			return GetSnippetsOccurencesAsync(snippetId, o => true);
		}

		public Task<List<SnippetOccurence>> GetSnippetsOccurencesAsync(int snippetId, Expression<Func<SnippetOccurence, bool>> filterFunction)
		{
			return db.SnippetsOccurences.Include(o => o.Submission).Where(o => o.SnippetId == snippetId).ToListAsync();
		}

		public async Task<List<Snippet>> GetSnippetsForSubmission(int submissionId)
		{
			return (await GetSnippetsOccurencesForSubmissionAsync(submissionId)).Select(o => o.Snippet).ToList();
		}
	}
}