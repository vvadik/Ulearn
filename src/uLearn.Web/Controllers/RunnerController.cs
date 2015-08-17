using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NLog.Internal;
using RunCsJob.Api;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class RunnerController : ApiController
	{
		private readonly UserSolutionsRepo userSolutionsRepo = new UserSolutionsRepo();
		private readonly CourseManager courseManager = WebCourseManager.Instance;
		/// <summary>
		/// Return list of submissions for testing purposes
		/// </summary>
		/// <param name="token"> Runner autherization token </param>
		/// <param name="count"> Count of submission </param>
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

		/// <summary>
		/// Get testing result
		/// </summary>
		/// <param name="token"> Runner autherization token </param>
		[HttpPost]
		[Route("PostResult")]
		public async Task PostResult([FromUri]string token, RunningResults result)
		{
			if (!ModelState.IsValid)
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			CheckRunner(token);

			await userSolutionsRepo.SaveResults(result);
		}

		/// <summary>
		/// Get testing results
		/// </summary>
		/// <param name="token"> Runner autherization token </param>
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
			var expectedToken = new ConfigurationManager().AppSettings["runnerToken"];
			if (expectedToken != token)
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
		}
	}
}
