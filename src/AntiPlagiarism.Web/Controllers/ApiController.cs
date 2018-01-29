using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Serilog;
using uLearn;

namespace AntiPlagiarism.Web.Controllers
{
	[Route("/Api")]
	public class ApiController : BaseController
	{
		private readonly ISubmissionsRepo submissionsRepo;
		private readonly ISnippetsRepo snippetsRepo;
		private readonly ITasksRepo tasksRepo;
		private readonly StatisticsParametersFinder statisticsParametersFinder;
		private readonly PlagiarismDetector plagiarismDetector;
		private readonly CodeUnitsExtractor codeUnitsExtractor;		
		private readonly AntiPlagiarismConfiguration configuration;

		private readonly SnippetsExtractor snippetsExtractor = new SnippetsExtractor();

		private readonly List<ITokenInSnippetConverter> tokenConverters = new List<ITokenInSnippetConverter>
		{
			new TokensKindsOnlyConverter(),
			new TokensKindsAndValuesConverter(),
		};

		public ApiController(
			ISubmissionsRepo submissionsRepo, ISnippetsRepo snippetsRepo, ITasksRepo tasksRepo,
			StatisticsParametersFinder statisticsParametersFinder,
			PlagiarismDetector plagiarismDetector,
			CodeUnitsExtractor codeUnitsExtractor,
			ILogger logger,
			IOptions<AntiPlagiarismConfiguration> configuration)
			: base(logger)
		{
			this.submissionsRepo = submissionsRepo;
			this.snippetsRepo = snippetsRepo;
			this.tasksRepo = tasksRepo;
			this.statisticsParametersFinder = statisticsParametersFinder;
			this.plagiarismDetector = plagiarismDetector;
			this.codeUnitsExtractor = codeUnitsExtractor;
			this.configuration = configuration.Value;
		}
		
		[HttpPost(nameof(AddSubmission))]
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
				"Добавляю новое решение {submissionId} по задаче {taskId}, автор {authorId}, язык {language}, доп. информация {additionalInfo}",
				submission.Id,
				parameters.TaskId,
				parameters.AuthorId,
				parameters.Language,
				parameters.AdditionalInfo
				);

			await ExtractSnippetsFromSubmissionAsync(submission);
			await CalculateTaskStatisticsParametersAsync(submission.TaskId);
			
			return Json(new AddSubmissionResult
			{
				SubmissionId = submission.Id,
			});
		}

		private int GetTokensCount(string code)
		{
			var codeUnits = codeUnitsExtractor.Extract(code);
			return codeUnits.Select(u => u.Tokens.Count).Sum();
		}

		[HttpGet(nameof(GetSubmissionPlagiarisms))]
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
			};
			
			return Json(result);
		}

		[HttpGet(nameof(GetAuthorPlagiarisms))]
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

			var submissions = await submissionsRepo.GetSubmissionsByAuthorAndTaskAsync(parameters.AuthorId, parameters.TaskId, parameters.LastSubmissionsCount);			
			var result = new GetAuthorPlagiarismsResult
			{
				SuspicionLevels = suspicionLevels,
			};
			foreach (var submission in submissions)
			{
				result.ResearchedSubmissions.Add(new ResearchedSubmission
				{
					SubmissionInfo = submission.GetSubmissionInfoForApi(),
					Plagiarisms = await plagiarismDetector.GetPlagiarismsAsync(submission, suspicionLevels),
					TokensPositions = plagiarismDetector.GetNeededTokensPositions(submission.ProgramText),
				});
			}

			return Json(result);
		}

		private async Task ExtractSnippetsFromSubmissionAsync(Submission submission)
		{
			logger.Information("Достаю сниппеты из решения {submissionId}, длина сниппетов: {tokensCount} токенов", submission.Id, configuration.SnippetTokensCount);
			var codeUnits = codeUnitsExtractor.Extract(submission.ProgramText);
			foreach (var codeUnit in codeUnits)
			{
				foreach (var tokenConverter in tokenConverters)
				{
					var snippets = snippetsExtractor.GetSnippets(codeUnit.Tokens, configuration.SnippetTokensCount, tokenConverter);
					foreach (var (index, snippet) in snippets.Enumerate())
					{
						await snippetsRepo.AddSnippetOccurenceAsync(submission, snippet, codeUnit.FirstTokenIndex + index);
					}
				}
			}
		}
		
		public async Task CalculateTaskStatisticsParametersAsync(Guid taskId)
		{
			var lastAuthorsIds = await submissionsRepo.GetLastAuthorsByTaskAsync(taskId, configuration.StatisticsAnalyzing.CountOfLastAuthorsForCalculatingMeanAndDeviation);
			var lastSubmissions = await submissionsRepo.GetLastSubmissionsByAuthorsForTaskAsync(taskId, lastAuthorsIds);

			var statisticsParameters = await statisticsParametersFinder.FindStatisticsParametersAsync(lastSubmissions);
			statisticsParameters.TaskId = taskId;
			await tasksRepo.SaveTaskStatisticsParametersAsync(statisticsParameters);
		}
		
		private async Task<SuspicionLevels> GetSuspicionLevelsAsync(Guid taskId)
		{
			var taskStatisticsParameters = await tasksRepo.FindTaskStatisticsParametersAsync(taskId);
			if (taskStatisticsParameters == null)
				return null;

			return new SuspicionLevels
			{
				FaintSuspicion = Math.Min(taskStatisticsParameters.Mean + 4 * taskStatisticsParameters.Deviation, 1),
				StrongSuspicion = Math.Min(taskStatisticsParameters.Mean + 6 * taskStatisticsParameters.Deviation, 1),
			};
		}
	}
}