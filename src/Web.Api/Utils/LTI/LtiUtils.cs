using System;
using System.Threading.Tasks;
using Database.Repos;
using log4net;
using LtiLibrary.Core.Outcomes.v1;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses.Slides;
using Ulearn.Web.Api.Controllers;

namespace Ulearn.Web.Api.Utils.LTI
{
	public static class LtiUtils
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(LtiUtils));

		public static async Task SubmitScore(string courseId, Slide slide, string userId, int score,
			ILtiRequestsRepo ltiRequestsRepo,
			ILtiConsumersRepo consumersRepo)
		{
			var ltiRequest = await ltiRequestsRepo.Find(courseId, userId, slide.Id);
			if (ltiRequest == null)
				throw new Exception("LtiRequest for user '" + userId + "' not found");

			var consumerSecret = (await consumersRepo.Find(ltiRequest.ConsumerKey)).Secret;

			log.Info($"Надо отправить результаты слайда {slide.Id} пользователя {userId} по LTI. Нашёл LtiRequest: {ltiRequest.JsonSerialize()}");
			UriBuilder uri;
			try
			{
				uri = new UriBuilder(ltiRequest.LisOutcomeServiceUrl);
			}
			catch (Exception e)
			{
				log.Error($"Неверный адрес отправки результатов по LTI: {ltiRequest.LisOutcomeServiceUrl}", e);
				throw;
			}

			if (uri.Host == "localhost")
			{
				uri.Host = "192.168.33.10";
				uri.Port = 80;
				uri.Scheme = "http";
			}

			var maxScore = BaseController.GetMaxScoreForUsersSlide(slide, true, false, false);
			var outputScore = score / (double)maxScore;
			log.Info($"Отправляю результаты на {ltiRequest.LisOutcomeServiceUrl}: {score} из {maxScore} ({outputScore})");

			/* Sometimes score is bigger then slide's MaxScore, i.e. in case of manual checking */
			if (score > maxScore)
				outputScore = 1;
			var result = OutcomesClient.PostScore(uri.ToString(), ltiRequest.ConsumerKey, consumerSecret,
				ltiRequest.LisResultSourcedId, outputScore);

			if (!result.IsValid)
				throw new Exception(uri + "\r\n\r\n" + result.Message);
		}
	}
}