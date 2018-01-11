using System.Threading.Tasks;
using AntiPlagiarism.Api.Models;
using AntiPlagiarism.Api.Models.Parameters;
using AntiPlagiarism.Api.Models.Results;
using AntiPlagiarism.Web.Database.Repos;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AntiPlagiarism.Web.Controllers
{
	[Route("/Api")]
	public class ApiController : BaseController
	{
		private readonly ISubmissionsRepo submissionsRepo;

		public ApiController(ISubmissionsRepo submissionsRepo, ILogger logger)
			: base(logger)
		{
			this.submissionsRepo = submissionsRepo;
		}
		
		[HttpPost("AddSubmission")]
		public async Task<IActionResult> AddSubmission(AddCodeApiParameters parameters)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var submission = await submissionsRepo.AddSubmissionAsync(client.Id, parameters.TaskId, parameters.AuthorId, parameters.Code, parameters.AdditionalInfo);
			
			return Json(new AddCodeApiResult
			{
				SubmissionId = submission.Id,
			});
		}
	}
}