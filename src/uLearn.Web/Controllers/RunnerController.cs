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
		private readonly UserSolutionsRepo userSolutionsRepo = new UserSolutionsRepo(new ULearnDb());
		private readonly CourseManager courseManager = WebCourseManager.Instance;

		[HttpGet]
		[Route("GetSubmissions")]
		public async Task<List<RunnerSubmission>> GetSubmissions([FromUri] string token, [FromUri] int count)
		{
			CheckRunner(token);
			var sw = Stopwatch.StartNew();
			while (true)
			{
				var repo = new UserSolutionsRepo(new ULearnDb());
				var exerciseCheckings = repo.GetUnhandledSubmissions(count);
				if (exerciseCheckings.Any() || sw.Elapsed > TimeSpan.FromSeconds(30))
				{
					return exerciseCheckings.Select(ToRunnerSubmission).ToList();
				}
				await repo.WaitAnyUnhandledSubmissions(TimeSpan.FromSeconds(10));
			}
		}

		private RunnerSubmission ToRunnerSubmission(UserExerciseSubmission submission)
		{
			if (submission.IsWebSubmission)
			{
				return new FileRunnerSubmission
				{
					Id = submission.Id.ToString(),
					Code = submission.SolutionCode.Text,
					Input = "",
					NeedRun = true
				};
			}
			var exerciseSlide = (ExerciseSlide)courseManager
				.GetCourse(submission.CourseId)
				.GetSlideById(submission.SlideId);
			courseManager.WaitWhileCourseIsLocked(submission.CourseId);
			return exerciseSlide.Exercise.CreateSubmition(
				submission.Id.ToString(),
				submission.SolutionCode.Text);
		}

		[HttpPost]
		[Route("PostResults")]
		public async Task PostResults([FromUri] string token, List<RunningResults> results)
		{
			if (!ModelState.IsValid)
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			CheckRunner(token);

			await userSolutionsRepo.SaveResults(results);
		}


		private void CheckRunner(string token)
		{
			var expectedToken = ConfigurationManager.AppSettings["runnerToken"];
			if (expectedToken != token)
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
		}
	}
}