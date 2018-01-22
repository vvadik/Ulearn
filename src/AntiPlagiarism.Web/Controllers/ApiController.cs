using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AntiPlagiarism.Api.Models.Parameters;
using AntiPlagiarism.Api.Models.Results;
using AntiPlagiarism.Web.CodeAnalyzing;
using AntiPlagiarism.Web.CodeAnalyzing.CSharp;
using AntiPlagiarism.Web.Database.Models;
using AntiPlagiarism.Web.Database.Repos;
using AntiPlagiarism.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
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
		private readonly AntiPlagiarismConfiguration configuration;

		private readonly CodeUnitsExtractor codeUnitsExtractor = new CodeUnitsExtractor();
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
			ILogger logger,
			IOptions<AntiPlagiarismConfiguration> configuration)
			: base(logger)
		{
			this.submissionsRepo = submissionsRepo;
			this.snippetsRepo = snippetsRepo;
			this.tasksRepo = tasksRepo;
			this.statisticsParametersFinder = statisticsParametersFinder;
			this.plagiarismDetector = plagiarismDetector;
			this.configuration = configuration.Value;
		}
		
		[HttpPost("AddSubmission")]
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

			await ExtractSnippetsFromSubmission(submission);
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

		[HttpGet("GetPlagiarisms")]
		public async Task<IActionResult> GetPlagiarisms(GetPlagiarismsParameters parameters)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			
			var submission = await submissionsRepo.FindSubmissionByIdAsync(parameters.SubmissionId);
			if (submission == null || submission.ClientId != client.Id)
				return Json(ApiError.Create("Invalid submission id"));

			var result = new GetPlagiarismsResult
			{
				Plagiarisms = await plagiarismDetector.GetPlagiarismsAsync(submission),
				TokensPositions = plagiarismDetector.GetNeededTokensPositions(submission.ProgramText),
			};
			
			return Json(result);
		}

		private async Task ExtractSnippetsFromSubmission(Submission submission)
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
						await snippetsRepo.AddSnippetOccurenceAsync(submission.Id, snippet, codeUnit.FirstTokenIndex + index);
					}
				}
			}
		}
		
		public async Task CalculateTaskStatisticsParametersAsync(Guid taskId)
		{
			/* TODO (andgein): move number 100 to config */
			var lastAuthorsIds = await submissionsRepo.GetLastAuthorsByTaskAsync(taskId, 100);
			var lastSubmissions = await submissionsRepo.GetLastSubmissionsByAuthorsForTaskAsync(taskId, lastAuthorsIds);

			var statisticsParameters = await statisticsParametersFinder.FindStatisticsParametersAsync(lastSubmissions);
			statisticsParameters.TaskId = taskId;
			await tasksRepo.SaveTaskStatisticsParametersAsync(statisticsParameters);
		}
	}
}