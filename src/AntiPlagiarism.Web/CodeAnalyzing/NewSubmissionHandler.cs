using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiPlagiarism.Web.Configuration;
using AntiPlagiarism.Web.Database;
using AntiPlagiarism.Web.Database.Models;
using AntiPlagiarism.Web.Database.Repos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace AntiPlagiarism.Web.CodeAnalyzing
{
	public class NewSubmissionHandler
	{
		private readonly AntiPlagiarismDb db;
		private readonly IWorkQueueRepo workQueueRepo;
		private readonly ISubmissionsRepo submissionsRepo;
		private readonly ITasksRepo tasksRepo;
		private readonly ISnippetsRepo snippetsRepo;
		private readonly SubmissionSnippetsExtractor submissionSnippetsExtractor;
		private readonly IServiceScopeFactory serviceScopeFactory;
		private readonly StatisticsParametersFinder statisticsParametersFinder;
		private readonly AntiPlagiarismConfiguration configuration;
		private readonly ILogger logger;
		
		public NewSubmissionHandler(AntiPlagiarismDb db, ILogger logger,
			ISubmissionsRepo submissionsRepo, ISnippetsRepo snippetsRepo, ITasksRepo tasksRepo, IWorkQueueRepo workQueueRepo,
			SubmissionSnippetsExtractor submissionSnippetsExtractor,
			IServiceScopeFactory serviceScopeFactory,
			IOptions<AntiPlagiarismConfiguration> configuration,
			StatisticsParametersFinder statisticsParametersFinder)
		{
			this.db = db;
			this.logger = logger;
			this.submissionsRepo = submissionsRepo;
			this.snippetsRepo = snippetsRepo;
			this.tasksRepo = tasksRepo;
			this.workQueueRepo = workQueueRepo;
			this.submissionSnippetsExtractor = submissionSnippetsExtractor;
			this.serviceScopeFactory = serviceScopeFactory;
			this.statisticsParametersFinder = statisticsParametersFinder;
			this.configuration = configuration.Value;
		}
		
		public async Task<bool> HandleNewSubmission()
		{
			var queueItem = await workQueueRepo.Take(QueueIds.NewSubmissionsQueue).ConfigureAwait(false);
			if (queueItem == null)
				return false;
			var submissionId = int.Parse(queueItem.ItemId);
			var submission = (await submissionsRepo.GetSubmissionsByIdsAsync(new List<int> { submissionId }).ConfigureAwait(false)).First();
			await ExtractSnippetsFromSubmissionAsync(submission).ConfigureAwait(false);
			await workQueueRepo.Remove(queueItem.Id);
			if (await NeedToRecalculateTaskStatistics(submission.ClientId, submission.TaskId).ConfigureAwait(false))
				await CalculateTaskStatisticsParametersAsync(submission.ClientId, submission.TaskId).ConfigureAwait(false);
			return true;
		}
		
		/* Определяет, пора ли пересчитывать параметры Mean и Deviation для заданной задачи.
		   В конфигурации для этого есть специальный параметр configuration.StatisticsAnalyzing.RecalculateStatisticsAfterSubmisionsCount.
		   Если он равен, например, 1000, то параметры будут пересчитываться после каждого тысячного решения по этой задаче.
		   Если решений пока меньше 1000, то параметры будут пересчитываться после 1-го, 2-го, 4-го, 8-го, 16-го, 32-го решения и так далее.
		 */
		private async Task<bool> NeedToRecalculateTaskStatistics(int clientId, Guid taskId)
		{
			var submissionsCount = await submissionsRepo.GetSubmissionsCountAsync(clientId, taskId).ConfigureAwait(false);
			var oldSubmissionsCount = (await tasksRepo.FindTaskStatisticsParametersAsync(taskId).ConfigureAwait(false))?.SubmissionsCount ?? 0;
			var recalculateStatisticsAfterSubmissionsCount = configuration.StatisticsAnalyzing.RecalculateStatisticsAfterSubmissionsCount;
			logger.Information($"Определяю, надо ли пересчитать статистические параметры задачи (TaskStatisticsParameters, параметры Mean и Deviation), задача {taskId}. " +
									$"Старое количество решений {oldSubmissionsCount}, новое {submissionsCount}, параметр recalculateStatisticsAfterSubmisionsCount={recalculateStatisticsAfterSubmissionsCount}.");

			if (submissionsCount < recalculateStatisticsAfterSubmissionsCount)
				return submissionsCount >= 2 * oldSubmissionsCount;

			return submissionsCount - oldSubmissionsCount >= recalculateStatisticsAfterSubmissionsCount;
		}

		public async Task ExtractSnippetsFromSubmissionAsync(Submission submission)
		{
			foreach (var (firstTokenIndex, snippet) in submissionSnippetsExtractor.ExtractSnippetsFromSubmission(submission))
				await snippetsRepo.AddSnippetOccurenceAsync(submission, snippet, firstTokenIndex).ConfigureAwait(false);
		}

		/// <returns>List of weights (numbers from [0, 1)) used for calculating mean and deviation for this task</returns>
		public async Task<List<double>> CalculateTaskStatisticsParametersAsync(int clientId, Guid taskId)
		{
			/* Create local submissions repo for preventing memory leaks */
			using (var scope = serviceScopeFactory.CreateScope())
			{
				db.DisableAutoDetectChanges();

				var localSubmissionsRepo = scope.ServiceProvider.GetService<ISubmissionsRepo>();

				logger.Information($"Пересчитываю статистические параметры задачи (TaskStatisticsParameters) по задаче {taskId}");
				var lastAuthorsIds = await localSubmissionsRepo.GetLastAuthorsByTaskAsync(clientId, taskId, configuration.StatisticsAnalyzing.CountOfLastAuthorsForCalculatingMeanAndDeviation).ConfigureAwait(false);
				var lastSubmissions = await localSubmissionsRepo.GetLastSubmissionsByAuthorsForTaskAsync(clientId, taskId, lastAuthorsIds).ConfigureAwait(false);
				var currentSubmissionsCount = await localSubmissionsRepo.GetSubmissionsCountAsync(clientId, taskId).ConfigureAwait(false);

				var (weights, statisticsParameters) = await statisticsParametersFinder.FindStatisticsParametersAsync(lastSubmissions).ConfigureAwait(false);
				logger.Information($"Новые статистические параметры задачи (TaskStatisticsParameters) по задаче {taskId}: Mean={statisticsParameters.Mean}, Deviation={statisticsParameters.Deviation}");
				statisticsParameters.TaskId = taskId;
				statisticsParameters.SubmissionsCount = currentSubmissionsCount;

				await tasksRepo.SaveTaskStatisticsParametersAsync(statisticsParameters).ConfigureAwait(false);

				return weights;
			}
		}
	}
}