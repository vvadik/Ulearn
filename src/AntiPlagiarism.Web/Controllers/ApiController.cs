using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntiPlagiarism.Api.Models.Parameters;
using AntiPlagiarism.Api.Models.Results;
using AntiPlagiarism.Web.CodeAnalyzing;
using AntiPlagiarism.Web.CodeAnalyzing.CSharp;
using AntiPlagiarism.Web.Configuration;
using AntiPlagiarism.Web.Database.Models;
using AntiPlagiarism.Web.Database.Repos;
using AntiPlagiarism.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace AntiPlagiarism.Web.Controllers
{
	[Route("/api")]
	public class ApiController : BaseController
	{
		private readonly ISubmissionsRepo submissionsRepo;
		private readonly ISnippetsRepo snippetsRepo;
		private readonly ITasksRepo tasksRepo;
		private readonly StatisticsParametersFinder statisticsParametersFinder;
		private readonly PlagiarismDetector plagiarismDetector;
		private readonly CodeUnitsExtractor codeUnitsExtractor;
		private readonly SubmissionSnippetsExtractor submissionSnippetsExtractor;
		private readonly IServiceScopeFactory serviceScopeFactory;
		private readonly AntiPlagiarismConfiguration configuration;

		private readonly SnippetsExtractor snippetsExtractor = new SnippetsExtractor();

		public ApiController(
			ISubmissionsRepo submissionsRepo, ISnippetsRepo snippetsRepo, ITasksRepo tasksRepo,
			StatisticsParametersFinder statisticsParametersFinder,
			PlagiarismDetector plagiarismDetector,
			CodeUnitsExtractor codeUnitsExtractor,
			SubmissionSnippetsExtractor submissionSnippetsExtractor,
			ILogger logger,
			IServiceScopeFactory serviceScopeFactory,
			IOptions<AntiPlagiarismConfiguration> configuration)
			: base(logger)
		{
			this.submissionsRepo = submissionsRepo;
			this.snippetsRepo = snippetsRepo;
			this.tasksRepo = tasksRepo;
			this.statisticsParametersFinder = statisticsParametersFinder;
			this.plagiarismDetector = plagiarismDetector;
			this.codeUnitsExtractor = codeUnitsExtractor;
			this.submissionSnippetsExtractor = submissionSnippetsExtractor;
			this.serviceScopeFactory = serviceScopeFactory;
			this.configuration = configuration.Value;
		}
		
		[HttpPost(Api.Urls.AddSubmission)]
		public async Task<IActionResult> AddSubmission(AddSubmissionParameters parameters)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (parameters.Code.Length > configuration.MaxCodeLength)
				return Json(ApiError.Create($"Code is too long. Maximum length is {configuration.MaxCodeLength} bytes"));

			var tokensCount = GetTokensCount(parameters.Code);
			var submission = await submissionsRepo.AddSubmissionAsync(
				client.Id,
				parameters.TaskId,
				parameters.AuthorId,
				parameters.Language,
				parameters.Code,
				tokensCount,
				parameters.AdditionalInfo
			);

			logger.Information(
				"Добавлено новое решение {submissionId} по задаче {taskId}, автор {authorId}, язык {language}, доп. информация {additionalInfo}",
				submission.Id,
				parameters.TaskId,
				parameters.AuthorId,
				parameters.Language,
				parameters.AdditionalInfo
				);

			await ExtractSnippetsFromSubmissionAsync(submission);
			if (await NeedToRecalculateTaskStatistics(client.Id, submission.TaskId))
				await CalculateTaskStatisticsParametersAsync(client.Id, submission.TaskId);
			
			return Json(new AddSubmissionResult
			{
				SubmissionId = submission.Id,
			});
		}

		/* Определяет, пора ли пересчитывать параметры Mean и Deviation для заданной задачи.
		   В конфигурации для этого есть специальный параметр configuration.StatisticsAnalyzing.RecalculateStatisticsAfterSubmisionsCount.
		   Если он равен, например, 1000, то параметры будут пересчитываться после каждого тысячного решения по этой задаче.
		   Но если решений пока меньше 1000, то параметры будут пересчитываться после 1-го, 2-го, 4-го, 8-го, 16-го, 32-го решения и так далее.
		 */
		private async Task<bool> NeedToRecalculateTaskStatistics(int clientId, Guid taskId)
		{
			var submissionsCount = await submissionsRepo.GetSubmissionsCountAsync(clientId, taskId);
			var oldSubmissionsCount = (await tasksRepo.FindTaskStatisticsParametersAsync(taskId))?.SubmissionsCount ?? 0;
			var recalculateStatisticsAfterSubmisionsCount = configuration.StatisticsAnalyzing.RecalculateStatisticsAfterSubmisionsCount;
			logger.Information($"Определяю, надо ли пересчитать статистические параметры задачи (TaskStatisticsParameters, параметры Mean и Deviation), задача {taskId}. " +
								$"Старое количество решений {oldSubmissionsCount}, новое {submissionsCount}, параметр recalculateStatisticsAfterSubmisionsCount={recalculateStatisticsAfterSubmisionsCount}.");

			if (submissionsCount < recalculateStatisticsAfterSubmisionsCount)
				return submissionsCount >= 2 * oldSubmissionsCount;

			return submissionsCount - oldSubmissionsCount >= recalculateStatisticsAfterSubmisionsCount;
		}

		private int GetTokensCount(string code)
		{
			var codeUnits = codeUnitsExtractor.Extract(code);
			return codeUnits.Select(u => u.Tokens.Count).Sum();
		}

		[HttpPost(Api.Urls.RebuildSnippetsForTask)]
		public async Task<IActionResult> RebuildSnippetsForTask(RebuildSnippetsForTaskParameters parameters)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			await snippetsRepo.RemoveSnippetsOccurencesForTaskAsync(parameters.TaskId);
			var submissions = await submissionsRepo.GetSubmissionsByTaskAsync(client.Id, parameters.TaskId);
			foreach (var submission in submissions)
			{
				await ExtractSnippetsFromSubmissionAsync(submission);
			}
			await CalculateTaskStatisticsParametersAsync(client.Id, parameters.TaskId);

			return Json(new RebuildSnippetsForTaskResult
			{
				SubmissionsIds = submissions.Select(s => s.Id).ToList(),
			});
		}

		[HttpPost(Api.Urls.RecalculateSnippetStatistics)]
		public async Task<IActionResult> RecalculateSnippetStatistics(RecalculateSnippetStatisticsParameters parameters)
		{
			var taskIds = await tasksRepo.GetTaskIds();
			
			if (parameters.FromTaskId.HasValue)
				taskIds = taskIds.Skip(taskIds.FindIndex(taskId => taskId == parameters.FromTaskId)).ToList();

			foreach (var taskId in taskIds)
			{
				await CalculateTaskStatisticsParametersAsync(client.Id, taskId);
				GC.Collect();
			}

			return Json(new RecalculateSnippetStatisticsResult
			{
				TaskIds = taskIds
			});
		}

		[HttpGet(Api.Urls.GetSubmissionPlagiarisms)]
		public async Task<IActionResult> GetSubmissionPlagiarisms(GetSubmissionPlagiarismsParameters parameters)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			
			var submission = await submissionsRepo.FindSubmissionByIdAsync(parameters.SubmissionId);
			if (submission == null || submission.ClientId != client.Id)
				return Json(ApiError.Create("Invalid submission id"));

			var suspicionLevels = await GetSuspicionLevelsAsync(submission.TaskId);
			if (suspicionLevels == null)
				return Json(ApiError.Create("Not enough statistics for defining suspicion levels"));
				
			var result = new GetSubmissionPlagiarismsResult
			{
				SubmissionInfo = submission.GetSubmissionInfoForApi(),
				Plagiarisms = await plagiarismDetector.GetPlagiarismsAsync(submission, suspicionLevels),
				TokensPositions = plagiarismDetector.GetNeededTokensPositions(submission.ProgramText),
				SuspicionLevels = suspicionLevels, 
				AnalyzedCodeUnits = GetAnalyzedCodeUnits(submission),
			};
			
			return Json(result);
		}

		[HttpGet(Api.Urls.GetAuthorPlagiarisms)]
		public async Task<IActionResult> GetAuthorPlagiarisms(GetAuthorPlagiarismsParameters parameters)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var maxLastSubmissionsCount = configuration.Actions.GetAuthorPlagiarisms.MaxLastSubmissionsCount;
			if (parameters.LastSubmissionsCount <= 0 || parameters.LastSubmissionsCount > maxLastSubmissionsCount)
				return Json(ApiError.Create(
					$"Invalid last_submissions_count. This value should be at least 1 and at most {maxLastSubmissionsCount}"
				));

			var suspicionLevels = await GetSuspicionLevelsAsync(parameters.TaskId);
			if (suspicionLevels == null)
				return Json(ApiError.Create("Not enough statistics for defining suspicion levels"));

			var submissions = await submissionsRepo.GetSubmissionsByAuthorAndTaskAsync(client.Id, parameters.AuthorId, parameters.TaskId, parameters.LastSubmissionsCount);			
			var result = new GetAuthorPlagiarismsResult
			{
				SuspicionLevels = suspicionLevels,
			};
			foreach (var submission in submissions)
			{
				using (var scope = serviceScopeFactory.CreateScope())
				{
					/* Create internal plagiarismDetector for preventing memory leaks */
					var internalPlagiarismDetector = scope.ServiceProvider.GetService<PlagiarismDetector>();
					
					result.ResearchedSubmissions.Add(new ResearchedSubmission
					{
						SubmissionInfo = submission.GetSubmissionInfoForApi(),
						Plagiarisms = await internalPlagiarismDetector.GetPlagiarismsAsync(submission, suspicionLevels),
						TokensPositions = internalPlagiarismDetector.GetNeededTokensPositions(submission.ProgramText),
						AnalyzedCodeUnits = GetAnalyzedCodeUnits(submission),
					});
				}
			}

			return Json(result);
		}

		private List<AnalyzedCodeUnit> GetAnalyzedCodeUnits(Submission submission)
		{
			var codeUnits = codeUnitsExtractor.Extract(submission.ProgramText);
			return codeUnits.Select(
				u => new AnalyzedCodeUnit
				{
					Name = u.Path.ToString(),
					FirstTokenIndex = u.FirstTokenIndex,
					TokensCount = u.Tokens.Count,
				}).ToList();
		}

		private async Task ExtractSnippetsFromSubmissionAsync(Submission submission)
		{
			foreach (var (firstTokenIndex, snippet) in submissionSnippetsExtractor.ExtractSnippetsFromSubmission(submission))
				await snippetsRepo.AddSnippetOccurenceAsync(submission, snippet, firstTokenIndex);
		}
		
		public async Task CalculateTaskStatisticsParametersAsync(int clientId, Guid taskId)
		{
			/* Create local submissions repo for preventing memory leaks */
			using (var scope = serviceScopeFactory.CreateScope())
			{
				var localSubmissionsRepo = scope.ServiceProvider.GetService<SubmissionsRepo>();

				logger.Information($"Пересчитываю статистические параметры задачи (TaskStatisticsParameters) по задаче {taskId}");
				var lastAuthorsIds = await localSubmissionsRepo.GetLastAuthorsByTaskAsync(clientId, taskId, configuration.StatisticsAnalyzing.CountOfLastAuthorsForCalculatingMeanAndDeviation);
				var lastSubmissions = await localSubmissionsRepo.GetLastSubmissionsByAuthorsForTaskAsync(clientId, taskId, lastAuthorsIds);
				var currentSubmissionsCount = await localSubmissionsRepo.GetSubmissionsCountAsync(clientId, taskId);

				var statisticsParameters = await statisticsParametersFinder.FindStatisticsParametersAsync(lastSubmissions);
				logger.Information($"Новые статистические параметры задачи (TaskStatisticsParameters) по задаче {taskId}: Mean={statisticsParameters.Mean}, Deviation={statisticsParameters.Deviation}");
				statisticsParameters.TaskId = taskId;
				statisticsParameters.SubmissionsCount = currentSubmissionsCount;
				await tasksRepo.SaveTaskStatisticsParametersAsync(statisticsParameters);
			}
		}
		
		private async Task<SuspicionLevels> GetSuspicionLevelsAsync(Guid taskId)
		{
			var taskStatisticsParameters = await tasksRepo.FindTaskStatisticsParametersAsync(taskId);
			if (taskStatisticsParameters == null)
				return null;
			
			var faintSuspicionCoefficient = configuration.StatisticsAnalyzing.FaintSuspicionCoefficient;
			var strongSuspicionCoefficient = configuration.StatisticsAnalyzing.StrongSuspicionCoefficient;
			var minFaintSuspicionLevel = configuration.StatisticsAnalyzing.MinFaintSuspicionLevel;
			var minStrongSuspicionLevel = configuration.StatisticsAnalyzing.MinStrongSuspicionLevel;

			return new SuspicionLevels
			{
				FaintSuspicion = GetSuspicionLevelWithThreshold(taskStatisticsParameters.Mean + faintSuspicionCoefficient * taskStatisticsParameters.Deviation, minFaintSuspicionLevel, 1),
				StrongSuspicion = GetSuspicionLevelWithThreshold(taskStatisticsParameters.Mean + strongSuspicionCoefficient * taskStatisticsParameters.Deviation, minStrongSuspicionLevel, 1),
			};
		}

		private static double GetSuspicionLevelWithThreshold(double value, double minValue, double maxValue)
		{
			if (minValue > maxValue)
				throw new ArgumentException("minValue should be less than maxValue");
			if (value < minValue)
				return minValue;
			if (value > maxValue)
				return maxValue;
			return value;
		}
	}
}