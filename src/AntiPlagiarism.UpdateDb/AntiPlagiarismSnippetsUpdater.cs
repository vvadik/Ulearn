using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiPlagiarism.Web.CodeAnalyzing;
using AntiPlagiarism.Web.Database.Models;
using AntiPlagiarism.Web.Database.Repos;
using Serilog;

namespace AntiPlagiarism.UpdateDb
{
	public class AntiPlagiarismSnippetsUpdater
	{
		private readonly ISubmissionsRepo submissionsRepo;
		private readonly ISnippetsRepo snippetsRepo;
		private readonly SubmissionSnippetsExtractor submissionSnippetsExtractor;
		private readonly ILogger logger;

		public AntiPlagiarismSnippetsUpdater(
			ISubmissionsRepo submissionsRepo,
			ISnippetsRepo snippetsRepo,
			SubmissionSnippetsExtractor submissionSnippetsExtractor,
			ILogger logger)
		{
			this.submissionsRepo = submissionsRepo;
			this.snippetsRepo = snippetsRepo;
			this.submissionSnippetsExtractor = submissionSnippetsExtractor;
			this.logger = logger;
		}

		public async Task UpdateAsync()
		{
			logger.Information("Начинаю обновлять информацию о сниппетах в базе данных");

			var startFromIndex = 0;
			var maxCount = 1000;
			while (true)
			{
				var submissions = await submissionsRepo.GetSubmissionsAsync(startFromIndex, maxCount);
				if (submissions.Count == 0)
					break;

				var firstSubmissionId = submissions.First().Id;
				var lastSubmissionId = submissions.Last().Id;
				logger.Information($"Получил {submissions.Count} следующих решений из базы данных. Идентификаторы решений от {firstSubmissionId} до {lastSubmissionId}");

				foreach (var submission in submissions)
				{
					try
					{
						await UpdateSnippetsFromSubmissionAsync(submission);
					}
					catch (Exception e)
					{
						logger.Error(e, $"Ошибка при обновлении списка сниппетов решения #{submission.Id}. Продолжаю работу со следующего решения");
					}
				}

				startFromIndex = lastSubmissionId + 1;
			}
			
			logger.Information("AntiPlagiarismSnippetsUpdater закончил свою работу");
		}

		private async Task UpdateSnippetsFromSubmissionAsync(Submission submission)
		{
			var occurences = new HashSet<Tuple<int, int>>(
				(await snippetsRepo.GetSnippetsOccurencesForSubmissionAsync(submission))
				.Select(o => Tuple.Create(o.SnippetId, o.FirstTokenIndex))
			);
			foreach (var (firstTokenIndex, snippet) in submissionSnippetsExtractor.ExtractSnippetsFromSubmission(submission))
			{
				var foundSnippet = await snippetsRepo.GetOrAddSnippetAsync(snippet);
				if (!occurences.Contains(Tuple.Create(foundSnippet.Id, firstTokenIndex)))
				{
					logger.Information($"Информация о сниппете #{foundSnippet.Id} в решении #{submission.Id} не найдена, добавляю");
					try
					{
						await snippetsRepo.AddSnippetOccurenceAsync(submission, foundSnippet, firstTokenIndex);
					}
					catch (Exception e)
					{
						logger.Error(e, $"Ошибка при добавлении сниппета #{foundSnippet.Id} в решении #{submission.Id}");
					}
				}
			}
		}
	}
}