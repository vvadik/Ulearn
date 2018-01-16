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
		private readonly AntiPlagiarismConfiguration configuration;

		private readonly CodeUnitsExtractor codeUnitsExtractor = new CodeUnitsExtractor();
		private readonly SnippetsExtractor snippetsExtractor = new SnippetsExtractor();

		private readonly List<ITokenInSnippetConverter> tokenConverters = new List<ITokenInSnippetConverter>
		{
			new TokensKindsOnlyConverter(),
			new TokensKindsAndValuesConverter(),
		};

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

		[HttpGet("GetPlagiarisms")]
		public async Task<IActionResult> GetPlagiarisms(GetPlagiarismsParameters parameters)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			
			var submission = await submissionsRepo.GetSubmissionByIdAsync(parameters.SubmissionId);
			if (submission.ClientId != client.Id)
				return Json(ApiError.Create("Invalid submission id"));

			/* Dictionaries by submission id and snippet type */
			var tokensMatchedInThisSubmission = new DefaultDictionary<Tuple<int, SnippetType>, HashSet<int>>();
			var tokensMatchedInOtherSubmissions = new DefaultDictionary<Tuple<int, SnippetType>, HashSet<int>>();
			
			var snippetsOccurences = await snippetsRepo.GetSnippetsOccurencesForSubmissionAsync(submission.Id);
			foreach (var snippetOccurence in snippetsOccurences)
			{
				var otherOccurences = await snippetsRepo.GetSnippetsOccurencesAsync(
					snippetOccurence.SnippetId,
					/* Filter only snippet occurences in submissions BY THIS client, THIS task and NOT BY THIS author */
					o => o.Submission.ClientId == submission.ClientId && o.Submission.TaskId == submission.TaskId && o.Submission.AuthorId != submission.AuthorId
				);

				var snippetType = snippetOccurence.Snippet.SnippetType;

				foreach (var otherOccurence in otherOccurences)
				{
					for (var i = 0; i < snippetOccurence.Snippet.TokensCount; i++)
					{
						var tokenIndexInThisSubmission = snippetOccurence.FirstTokenIndex + i;
						var tokenIndexInOtherSubmission = otherOccurence.FirstTokenIndex + i;
						tokensMatchedInThisSubmission[Tuple.Create(otherOccurence.SubmissionId, snippetType)].Add(tokenIndexInThisSubmission);
						tokensMatchedInOtherSubmissions[Tuple.Create(otherOccurence.SubmissionId, snippetType)].Add(tokenIndexInOtherSubmission);
					}
				}
			}

			var plagiateSubmissionIds = tokensMatchedInOtherSubmissions.Keys.Select(tuple => tuple.Item1).ToList();
			var plagiateSubmissions = await submissionsRepo.GetSubmissionsByIdsAsync(plagiateSubmissionIds);

			var allSnippetTypes = Enum.GetValues(typeof(SnippetType)).Cast<SnippetType>().ToList();
			var thisSubmissionLength = codeUnitsExtractor.Extract(submission.ProgramText).Select(u => u.Tokens.Count).Sum();
			var result = new GetPlagiarismsResult();
			foreach (var plagiateSubmission in plagiateSubmissions)
			{
				var totalUnion = 0;
				foreach (var snippetType in allSnippetTypes)
				{
					var submissionIdWithSnippetType = Tuple.Create(plagiateSubmission.Id, snippetType);
					if (!tokensMatchedInThisSubmission.ContainsKey(submissionIdWithSnippetType))
						continue;
					
					totalUnion += tokensMatchedInThisSubmission[submissionIdWithSnippetType].Count;
					totalUnion += tokensMatchedInOtherSubmissions[submissionIdWithSnippetType].Count;
				}

				/* TODO (andgein): store submissionLengths somewhere in the database */
				var plagiateSubmissionLength = codeUnitsExtractor.Extract(plagiateSubmission.ProgramText).Select(u => u.Tokens.Count).Sum();
				var totalLength = thisSubmissionLength + plagiateSubmissionLength;
				var weight = ((double)totalUnion) / totalLength;
				
				result.Plagiarisms.Add(new Plagiarism
				{
					Submission = new PlagiateSubmission
					{
						Id = plagiateSubmission.Id,
						TaskId = plagiateSubmission.TaskId,
						AuthorId = plagiateSubmission.AuthorId,
						Code = plagiateSubmission.ProgramText,
						AdditionalInfo = plagiateSubmission.AdditionalInfo,
					},
					Weight = weight,
				});
			}
			
			return Json(result);
		}

		private async Task ExtractSnippetsFromSubmission(Submission submission)
		{
			logger.Information("Extracting snippets from submission {submissionId}, snippet tokens count = {tokensCount}", submission.Id, configuration.SnippetTokensCount);
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
	}
}