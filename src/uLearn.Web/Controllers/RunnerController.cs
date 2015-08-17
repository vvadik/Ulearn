using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using RunCsJob.Api;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class RunnerController : ApiController
	{
		private readonly UserSolutionsRepo userSolutionsRepo = new UserSolutionsRepo();
		private readonly CourseManager courseManager = WebCourseManager.Instance;

		[HttpGet]
		[Route("GetSubmissions")]
		public async Task<List<RunnerSubmition>> GetSubmissions([FromUri] string token, [FromUri] int count)
		{
			CheckRunner(token);
			var sw = Stopwatch.StartNew();
			while (true)
			{
				var repo = new UserSolutionsRepo();
				var submissions = repo.GetUnhandled(count);
				if (submissions.Any() || sw.Elapsed > TimeSpan.FromSeconds(30))
					return submissions.Select(ToRunnerSubmition).ToList();
				await repo.WaitUnhandled(TimeSpan.FromSeconds(10));
			}
		}

		private RunnerSubmition ToRunnerSubmition(UserSolution details)
		{
			return new RunnerSubmition
			{
				Id = details.Id.ToString(),
				Code = Utils.GetSource(
					details.CourseId, 
					details.SlideId, 
					courseManager, 
					details.SolutionCode.Text
					),
				Input = "",
				NeedRun = true
			};
		}

		[HttpPost]
		[Route("PostResult")]
		public async Task PostResult([FromUri]string token, RunningResults result)
		{
			if (!ModelState.IsValid)
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			CheckRunner(token);

			await userSolutionsRepo.SaveResults(result);
		}

		[HttpPost]
		[Route("PostResults")]
		public async Task PostResults([FromUri]string token, List<RunningResults> results)
		{
			if (!ModelState.IsValid)
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			CheckRunner(token);

			await userSolutionsRepo.SaveAllResults(results);
		}


		private void CheckRunner(string token)
		{
			var expectedToken = ConfigurationManager.AppSettings["runnerToken"];
			if (expectedToken != token)
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
		}
	}
}
