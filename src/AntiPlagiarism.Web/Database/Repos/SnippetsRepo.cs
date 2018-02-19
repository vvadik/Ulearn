using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AntiPlagiarism.Api.Models;
using AntiPlagiarism.Web.Database.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AntiPlagiarism.Web.Database.Repos
{
	public interface ISnippetsRepo
	{
		Task<Snippet> AddSnippetAsync(int tokensCount, SnippetType type, int hash);
		Task<bool> IsSnippetExistsAsync(int tokensCount, SnippetType type, int hash);
		Task<SnippetOccurence> AddSnippetOccurenceAsync(Submission submission, Snippet snippet, int firstTokenIndex);
		Task<List<SnippetOccurence>> GetSnippetsOccurencesForSubmissionAsync(Submission submission, int maxCount);
		Task<Dictionary<int, SnippetStatistics>> GetSnippetsStatisticsAsync(int clientId, Guid taskId, IEnumerable<int> snippetsIds);
		Task<List<SnippetOccurence>> GetSnippetsOccurencesAsync(int snippetId);
		Task<List<SnippetOccurence>> GetSnippetsOccurencesAsync(int snippetId, Expression<Func<SnippetOccurence, bool>> filterFunction);
		Task RemoveSnippetsOccurencesForTaskAsync(Guid taskId);
	}

	public class SnippetsRepo : ISnippetsRepo
	{
		private readonly AntiPlagiarismDb db;
		private readonly ILogger logger;

		public SnippetsRepo(AntiPlagiarismDb db, ILogger logger)
		{
			this.db = db;
			this.logger = logger;
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

		public async Task<SnippetOccurence> AddSnippetOccurenceAsync(Submission submission, Snippet snippet, int firstTokenIndex)
		{
			var foundSnippet = await GetOrAddSnippetAsync(snippet);
			var snippetOccurence = new SnippetOccurence
			{
				SubmissionId = submission.Id,
				Snippet = foundSnippet,
				FirstTokenIndex = firstTokenIndex,
			};
			await db.SnippetsOccurences.AddAsync(snippetOccurence);
			await db.SaveChangesAsync();

			var snippetStatistics = await GetOrAddSnippetStatisticsAsync(foundSnippet, submission.TaskId, submission.ClientId);
			snippetStatistics.AuthorsCount = await db.SnippetsOccurences.Include(o => o.Submission)
				.Where(o => o.SnippetId == foundSnippet.Id &&
							o.Submission.ClientId == submission.ClientId &&
							o.Submission.TaskId == submission.TaskId)
				.Select(o => o.Submission.AuthorId)
				.Distinct()
				.CountAsync();
			await db.SaveChangesAsync();
			
			return snippetOccurence;
		}

		private async Task<SnippetStatistics> GetOrAddSnippetStatisticsAsync(Snippet snippet, Guid taskId, int clientId)
		{
			using (var transaction = db.Database.BeginTransaction())
			{
				var foundStatistics = await db.SnippetsStatistics.SingleOrDefaultAsync(
					s => s.SnippetId == snippet.Id &&
						s.TaskId == taskId &&
						s.ClientId == clientId
						);
				if (foundStatistics != null)
					return foundStatistics;

				var addedStatistics = await db.SnippetsStatistics.AddAsync(new SnippetStatistics
				{
					SnippetId = snippet.Id,					
					ClientId = clientId,
					TaskId = taskId,
				});
				await db.SaveChangesAsync();
				transaction.Commit();

				return addedStatistics.Entity;
			}
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

		private async Task SomeTests(Submission submission, int maxCount)
		{
			logger.Debug("Запускаю тесты, чтобы найти косяк");
			
			logger.Debug("Тест 1");
			var list1 = db.SnippetsStatistics.Where(s => s.TaskId == submission.TaskId && s.ClientId == submission.ClientId).ToList();
			logger.Debug("Тест 1 закончен");
			logger.Debug($"Результат теста 1: {string.Join(", ", list1)}");
			
			logger.Debug("Тест 2");
			var list2 = await db.SnippetsStatistics.Where(s => s.TaskId == submission.TaskId && s.ClientId == submission.ClientId).ToListAsync();
			logger.Debug("Тест 2 закончен");
			logger.Debug($"Результат теста 2: {string.Join(", ", list2)}");
			
			var selectedSnippetsStatistics = db.SnippetsStatistics.Where(s => s.TaskId == submission.TaskId && s.ClientId == submission.ClientId);
			
			logger.Debug("Тест 3");
			var list3 = db.SnippetsOccurences.Include(o => o.Snippet)
				.Join(selectedSnippetsStatistics, o => o.SnippetId, s => s.SnippetId, (occurence, statistics) => new { occurence, statistics })
				.ToList();
			logger.Debug("Тест 3 закончен");
			logger.Debug($"Результат теста 3: {string.Join(", ", list3)}");

			logger.Debug("Тест 4");
			var list4 = db.SnippetsOccurences.Include(o => o.Snippet)
				.Join(selectedSnippetsStatistics, o => o.SnippetId, s => s.SnippetId, (occurence, statistics) => new { occurence, statistics })
				.Where(o => o.occurence.SubmissionId == submission.Id)
				.ToList();
			logger.Debug("Тест 4 закончен");
			logger.Debug($"Результат теста 4: {string.Join(", ", list4)}");			
			
			logger.Debug("Тест 5");
			var list5 = db.SnippetsOccurences.Include(o => o.Snippet)
				.Join(selectedSnippetsStatistics, o => o.SnippetId, s => s.SnippetId, (occurence, statistics) => new { occurence, statistics })
				.Where(o => o.occurence.SubmissionId == submission.Id)
				.OrderBy(o => o.statistics.AuthorsCount)
				.ToList();
			logger.Debug("Тест 5 закончен");
			logger.Debug($"Результат теста 5: {string.Join(", ", list5)}");
			
			logger.Debug("Тест 6");
			var list6 = db.SnippetsOccurences.Include(o => o.Snippet)
				.Join(selectedSnippetsStatistics, o => o.SnippetId, s => s.SnippetId, (occurence, statistics) => new { occurence, statistics })
				.Where(o => o.occurence.SubmissionId == submission.Id)
				.OrderBy(o => o.statistics.AuthorsCount)
				.Take(maxCount)
				.ToList();
			logger.Debug("Тест 6 закончен");
			logger.Debug($"Результат теста 6: {string.Join(", ", list6)}");
			
			logger.Debug("Тест 7");
			var list7 = db.SnippetsOccurences.Include(o => o.Snippet)
				.Join(selectedSnippetsStatistics, o => o.SnippetId, s => s.SnippetId, (occurence, statistics) => new { occurence, statistics })
				.Where(o => o.occurence.SubmissionId == submission.Id)
				.OrderBy(o => o.statistics.AuthorsCount)
				.Take(maxCount)
				.ToList()
				.Select(o => o?.occurence)
				.ToList();
			logger.Debug("Тест 7 закончен");
			logger.Debug($"Результат теста 7: {string.Join(", ", list7)}");
			
			logger.Debug("Тест 8");
			var list8 = db.SnippetsOccurences.Include(o => o.Snippet)
				.Join(selectedSnippetsStatistics, o => o.SnippetId, s => s.SnippetId, (occurence, statistics) => new { occurence, statistics })
				.Where(o => o.occurence.SubmissionId == submission.Id)
				.OrderBy(o => o.statistics.AuthorsCount)
				.Take(maxCount)
				.ToList()
				.Select(o => o.occurence)
				.ToList();
			logger.Debug("Тест 8 закончен");
			logger.Debug($"Результат теста 8: {string.Join(", ", list8)}");
		}
		
		public async Task<List<SnippetOccurence>> GetSnippetsOccurencesForSubmissionAsync(Submission submission, int maxCount)
		{
			await SomeTests(submission, maxCount);
			
			var selectedSnippetsStatistics = db.SnippetsStatistics.Where(s => s.TaskId == submission.TaskId && s.ClientId == submission.ClientId);
			return await db.SnippetsOccurences.Include(o => o.Snippet)
				.Join(selectedSnippetsStatistics, o => o.SnippetId, s => s.SnippetId, (occurence, statistics) => new { occurence, statistics })
				.Where(o => o.occurence.SubmissionId == submission.Id)
				.OrderBy(o => o.statistics.AuthorsCount)
				.Take(maxCount)
				.Select(o => o.occurence)
				.ToListAsync();
		}

		public Task<Dictionary<int, SnippetStatistics>> GetSnippetsStatisticsAsync(int clientId, Guid taskId, IEnumerable<int> snippetsIds)
		{
			return db.SnippetsStatistics
				.Where(s => s.ClientId == clientId && s.TaskId == taskId && snippetsIds.Contains(s.SnippetId))
				.ToDictionaryAsync(s => s.SnippetId);
		}

		public Task<List<SnippetOccurence>> GetSnippetsOccurencesAsync(int snippetId)
		{
			return GetSnippetsOccurencesAsync(snippetId, o => true);
		}

		public Task<List<SnippetOccurence>> GetSnippetsOccurencesAsync(int snippetId, Expression<Func<SnippetOccurence, bool>> filterFunction)
		{
			return db.SnippetsOccurences.Include(o => o.Submission).Where(o => o.SnippetId == snippetId).Where(filterFunction).ToListAsync();
		}

		public async Task RemoveSnippetsOccurencesForTaskAsync(Guid taskId)
		{
			db.SnippetsOccurences.RemoveRange(
				await db.SnippetsOccurences.Include(o => o.Submission).Where(o => o.Submission.TaskId == taskId).ToListAsync()
			);
		}

		public async Task<List<Snippet>> GetSnippetsForSubmission(Submission submission, int maxCount)
		{
			return (await GetSnippetsOccurencesForSubmissionAsync(submission, maxCount)).Select(o => o.Snippet).ToList();
		}
	}
}