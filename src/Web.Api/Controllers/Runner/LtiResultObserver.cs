using System;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Ulearn.Core.Courses.Slides.Exercises;
using Ulearn.Core.RunCheckerJobApi;
using Ulearn.Web.Api.Utils.LTI;
using Vostok.Logging.Abstractions;

namespace Ulearn.Web.Api.Controllers.Runner
{
	public class LtiResultObserver : IResultObserver
	{
		private readonly IWebCourseManager courseManager;
		private readonly ILtiConsumersRepo ltiConsumersRepo;
		private readonly ILtiRequestsRepo ltiRequestsRepo;
		private readonly IVisitsRepo visitsRepo;
		private static ILog log => LogProvider.Get().ForContext(typeof(LtiResultObserver));

		public LtiResultObserver(IWebCourseManager courseManager, ILtiConsumersRepo ltiConsumersRepo,
			ILtiRequestsRepo ltiRequestsRepo, IVisitsRepo visitsRepo)
		{
			this.courseManager = courseManager;
			this.ltiConsumersRepo = ltiConsumersRepo;
			this.ltiRequestsRepo = ltiRequestsRepo;
			this.visitsRepo = visitsRepo;
		}

		public async Task ProcessResult(UserExerciseSubmission submission, RunningResults result)
		{
			var ltiRequestJson = await ltiRequestsRepo.Find(submission.CourseId, submission.UserId, submission.SlideId);
			if (ltiRequestJson != null)
			{
				try
				{
					var exerciseSlide = (await courseManager.FindCourseAsync(submission.CourseId)).FindSlideById(submission.SlideId, true) as ExerciseSlide;
					var score = await visitsRepo.GetScore(submission.CourseId, submission.SlideId, submission.UserId);
					await LtiUtils.SubmitScore(exerciseSlide, submission.UserId, score, ltiRequestJson, ltiConsumersRepo);
				}
				catch (Exception e)
				{
					log.Error(e, "Мы не смогли отправить баллы на образовательную платформу");
				}
			}
		}
	}
}