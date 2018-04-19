using System;
using System.Collections.Generic;
using System.Data;
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
		Task<List<SnippetOccurence>> GetSnippetsOccurencesForSubmissionAsync(Submission submission);
		Task<Dictionary<int, SnippetStatistics>> GetSnippetsStatisticsAsync(int clientId, Guid taskId, IEnumerable<int> snippetsIds);
		Task<List<SnippetOccurence>> GetSnippetsOccurencesAsync(int snippetId);
		Task<List<SnippetOccurence>> GetSnippetsOccurencesAsync(int snippetId, Expression<Func<SnippetOccurence, bool>> filterFunction);
		Task RemoveSnippetsOccurencesForTaskAsync(Guid taskId);
		Task<Snippet> GetOrAddSnippetAsync(Snippet snippet);
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
			logger.Information($"Сохраняю в базу информацию о сниппете {snippet} в решении #{submission.Id} в позиции {firstTokenIndex}");
			logger.Information($"Ищу сниппет {snippet} в базе (или создаю новый)");
			var foundSnippet = await GetOrAddSnippetAsync(snippet);
			logger.Information($"Сниппет в базе имеет номер {foundSnippet.Id}");
			var snippetOccurence = new SnippetOccurence
			{
				SubmissionId = submission.Id,
				Snippet = foundSnippet,
				FirstTokenIndex = firstTokenIndex,
			};
			logger.Information($"Добавляю в базу объект {snippetOccurence}");
			
			
			DisableAutoDetectChanges();
			
			/* ...and use non-async Add() here because of perfomance issues with async versions */
			db.SnippetsOccurences.Add(snippetOccurence);
			db.Entry(snippetOccurence).State = EntityState.Added;
			await db.SaveChangesAsync();
		
			EnableAutoDetectChanges();
			
			
			logger.Information($"Добавил. Пересчитываю статистику сниппета (количество авторов, у которых он встречается)");
			var snippetStatistics = await GetOrAddSnippetStatisticsAsync(foundSnippet, submission.TaskId, submission.ClientId);

			
			DisableAutoDetectChanges();
			
			logger.Information($"Старая статистика сниппета {foundSnippet}: {snippetStatistics}");
			/* Use non-async Add() here because of perfomance issues with async versions */
			snippetStatistics.AuthorsCount = db.SnippetsOccurences.Include(o => o.Submission)
				.Where(o => o.SnippetId == foundSnippet.Id &&
							o.Submission.ClientId == submission.ClientId &&
							o.Submission.TaskId == submission.TaskId)
				.Select(o => o.Submission.AuthorId)
				.Distinct()
				.Count();
			db.Entry(snippetStatistics).State = EntityState.Modified;
			logger.Information($"Количество авторов, у которых встречается сниппет {foundSnippet} — {snippetStatistics.AuthorsCount}");
			await db.SaveChangesAsync();
		
			EnableAutoDetectChanges();

			
			logger.Information($"Закончил сохранение в базу информации о сниппете {foundSnippet} в решении #{submission.Id} в позиции {firstTokenIndex}");
			return snippetOccurence;
		}

		private async Task<SnippetStatistics> GetOrAddSnippetStatisticsAsync(Snippet snippet, Guid taskId, int clientId)
		{
			DisableAutoDetectChanges();
			try
			{
				using (var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable))
				{
					var foundStatistics = await db.SnippetsStatistics.SingleOrDefaultAsync(
						s => s.SnippetId == snippet.Id &&
							s.TaskId == taskId &&
							s.ClientId == clientId
					);
					if (foundStatistics != null)
						return foundStatistics;

					var addedStatistics = db.SnippetsStatistics.Add(new SnippetStatistics
					{
						SnippetId = snippet.Id,
						ClientId = clientId,
						TaskId = taskId,
					});
					addedStatistics.State = EntityState.Added;
					await db.SaveChangesAsync();
					transaction.Commit();

					return addedStatistics.Entity;
				}
			}
			finally
			{
				EnableAutoDetectChanges();
			}
		}

		/* We stands with perfomance issue on EF Core: https://github.com/aspnet/EntityFrameworkCore/issues/11680
  		   So we decided to disable AutoDetectChangesEnabled temporary for some queries */
		private void DisableAutoDetectChanges()
		{
			logger.Information("Выключаю AutoDetectChangesEnabled ");
			db.ChangeTracker.AutoDetectChangesEnabled = false;
		}

		private void EnableAutoDetectChanges()
		{
			logger.Information("Включаю AutoDetectChangesEnabled обратно");
			db.ChangeTracker.AutoDetectChangesEnabled = true;
		}

		public async Task<Snippet> GetOrAddSnippetAsync(Snippet snippet)
		{
			using (var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable))
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
		
		public async Task<List<SnippetOccurence>> GetSnippetsOccurencesForSubmissionAsync(Submission submission, int maxCount)
		{
			if (maxCount == 0)
				return await db.SnippetsOccurences.Where(o => o.SubmissionId == submission.Id).ToListAsync();
			
			var selectedSnippetsStatistics = db.SnippetsStatistics.Where(s => s.TaskId == submission.TaskId && s.ClientId == submission.ClientId);
			return (await db.SnippetsOccurences.Include(o => o.Snippet)
				.Join(selectedSnippetsStatistics, o => o.SnippetId, s => s.SnippetId, (occurence, statistics) => new { occurence, statistics })
				.Where(o => o.occurence.SubmissionId == submission.Id)
				.OrderBy(o => o.statistics.AuthorsCount)
				.Take(maxCount)
				.ToListAsync())
				.Select(o => o.occurence)
				.ToList();
		}

		public Task<List<SnippetOccurence>> GetSnippetsOccurencesForSubmissionAsync(Submission submission)
		{
			return GetSnippetsOccurencesForSubmissionAsync(submission, 0);
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