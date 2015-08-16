using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using LtiLibrary.Core.Outcomes.v1;
using Microsoft.AspNet.Identity;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class ExerciseController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly UserSolutionsRepo solutionsRepo = new UserSolutionsRepo();
		private readonly VisitersRepo visitersRepo = new VisitersRepo();
		private readonly ConsumersRepo consumersRepo = new ConsumersRepo();
		private readonly LtiRequestsRepo ltiRequestsRepo = new LtiRequestsRepo();

		private readonly static TimeSpan executionTimeout = TimeSpan.FromSeconds(30);

		public ExerciseController()
			: this(WebCourseManager.Instance)
		{
		}

		public ExerciseController(CourseManager courseManager)
		{
			this.courseManager = courseManager;
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> RunSolution(string courseId, int slideIndex = 0, bool isLti = false)
		{
			var code = Request.InputStream.GetString();
			if (code.Length > TextsRepo.MAX_TEXT_SIZE)
			{
				return Json(new RunSolutionResult
				{
					IsCompileError = true,
					CompilationError = "Слишком большой код."
				});
			}
			var exerciseSlide = (ExerciseSlide)courseManager.GetCourse(courseId).Slides[slideIndex];
			
			var result = await CheckSolution(courseId, exerciseSlide, code);
			if (isLti)
				SubmitScore(exerciseSlide);
			return Json(result);
		}

		private void SubmitScore(Slide slide)
		{
			var userId = User.Identity.GetUserId();

			var ltiRequest = ltiRequestsRepo.Find(userId, slide.Id);
			if (ltiRequest == null)
				throw new Exception("LtiRequest for user '" + userId + "' not found");

			var consumerSecret = consumersRepo.Find(ltiRequest.ConsumerKey).Secret;

			var score = visitersRepo.GetScore(slide.Id, userId);

			// TODO: fix outcome address in local edx (no localhost and no https)
			var uri = new UriBuilder(ltiRequest.LisOutcomeServiceUrl);
			if (uri.Host == "localhost")
			{
				uri.Host = "192.168.33.10";
				uri.Port = 80;
				uri.Scheme = "http";
			}

			var result = OutcomesClient.PostScore(uri.ToString(), ltiRequest.ConsumerKey, consumerSecret,
				ltiRequest.LisResultSourcedId, score / (double)slide.MaxScore);

			if (!result.IsValid)
				throw new Exception(uri + "\r\n\r\n" + result.Message);
		}

		private async Task<RunSolutionResult> CheckSolution(string courseId, ExerciseSlide exerciseSlide, string code)
		{
			var exerciseBlock = exerciseSlide.Exercise;
			var solution = exerciseBlock.Solution.BuildSolution(code);
			if (solution.HasErrors)
				return new RunSolutionResult { IsCompileError = true, CompilationError = solution.ErrorMessage, ExecutionServiceName = "uLearn" };
			if (solution.HasStyleIssues)
				return new RunSolutionResult { IsStyleViolation = true, CompilationError = solution.StyleMessage, ExecutionServiceName = "uLearn" };

			var submissionDetails = await solutionsRepo.RunUserSolution(
				courseId, exerciseSlide.Id, User.Identity.GetUserId(), 
				code, null, null, false, "uLearn", 
				GenerateSubmissionName(exerciseSlide), executionTimeout
			);

			if (submissionDetails == null)
				return new RunSolutionResult
				{
					IsCompillerFailure = true,
					CompilationError = "Ой-ой, штуковина, которая проверяет решения сломалась (или просто устала). Попробуйте отправить решение позже (когда она немного отдохнет).",
					ExecutionServiceName = "uLearn"
				};
			var output = submissionDetails.Output.Text;
			var expectedOutput = exerciseBlock.ExpectedOutput.NormalizeEoln();
			var isRightAnswer = submissionDetails.GetVerdict() == "Accepted" && output.Equals(expectedOutput);

			await visitersRepo.AddSolutionAttempt(exerciseSlide.Id, User.Identity.GetUserId(), isRightAnswer);

			return new RunSolutionResult
			{
				IsCompileError = submissionDetails.IsCompilationError,
				CompilationError = submissionDetails.CompilationError.Text,
				IsRightAnswer = isRightAnswer,
				ExpectedOutput = exerciseBlock.HideExpectedOutputOnError ? null : expectedOutput,
				ActualOutput = output,
				ExecutionServiceName = submissionDetails.ExecutionServiceName
			};
		}

		private string GenerateSubmissionName(Slide exerciseSlide)
		{
			return string.Format("{0}: {1} - {2}", User.Identity.Name, exerciseSlide.Info.UnitName, exerciseSlide.Title);
		}
	}
}
