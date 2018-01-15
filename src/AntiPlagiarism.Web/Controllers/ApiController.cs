using System.Threading.Tasks;
using AntiPlagiarism.Api.Models;
using AntiPlagiarism.Api.Models.Parameters;
using AntiPlagiarism.Api.Models.Results;
using AntiPlagiarism.Web.CodeAnalyzing;
using AntiPlagiarism.Web.CodeAnalyzing.CSharp;
using AntiPlagiarism.Web.Database.Models;
using AntiPlagiarism.Web.Database.Repos;
using AntiPlagiarism.Web.Extensions;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;

namespace AntiPlagiarism.Web.Controllers
{
	[Route("/Api")]
	public class ApiController : BaseController
	{
		private readonly ISubmissionsRepo submissionsRepo;
		private readonly ISnippetsRepo snippetsRepo;
		private readonly AntiPlagiarismConfiguration configuration;

		private readonly CodeUnitsExtractor codeUnitsExtractor = new CodeUnitsExtractor();
		private readonly SnippetsExtractor snippetsExtractor = new SnippetsExtractor();
		private readonly ITokenInSnippetConverter tokenConverter = new TokensKindsOnlyConverter();

		public ApiController(ISubmissionsRepo submissionsRepo, ISnippetsRepo snippetsRepo, ILogger logger, IOptions<AntiPlagiarismConfiguration> configuration)
			: base(logger)
		{
			this.submissionsRepo = submissionsRepo;
			this.snippetsRepo = snippetsRepo;
			this.configuration = configuration.Value;
		}
		
		[HttpPost("AddSubmission")]
		public async Task<IActionResult> AddSubmission(AddSubmissionParameters parameters)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			
			var submission = await submissionsRepo.AddSubmissionAsync(
				client.Id,
				parameters.TaskId,
				parameters.AuthorId,
				parameters.Language,
				parameters.Code,
				parameters.AdditionalInfo
			);

			logger.Information(
				"Added new submission {submissionId} by task {taskId}, author {authorId}, language {language}, additional info {additionalInfo}",
				submission.Id,
				parameters.TaskId,
				parameters.AuthorId,
				parameters.Language,
				parameters.AdditionalInfo
				);

			await ExtractSnippetsFromSubmission(submission);
			
			return Json(new AddSubmissionResult
			{
				SubmissionId = submission.Id,
			});
		}

		private async Task ExtractSnippetsFromSubmission(Submission submission)
		{
			logger.Information("Extracting snippets from submission {submissionId}, snippet tokens count = {tokensCount}", submission.Id, configuration.SnippetTokensCount);
			var codeUnits = codeUnitsExtractor.Extract(submission.ProgramText);
			foreach (var codeUnit in codeUnits)
			{
				var snippets = snippetsExtractor.GetSnippets(codeUnit.Tokens, configuration.SnippetTokensCount, tokenConverter);
				foreach (var (index, snippet) in snippets.Enumerate())
				{
					await snippetsRepo.AddSnippetOccurenceAsync(submission.Id, snippet, codeUnit.FirstTokenIndex + index);
				}
			}
			
		}
	}
}