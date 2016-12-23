using System;
using LtiLibrary.Core.Outcomes.v1;
using uLearn.Web.DataContexts;

namespace uLearn.Web.LTI
{
	public static class LtiUtils
	{
		public static void SubmitScore(Slide slide, string userId)
		{
			var ltiRequestsRepo = new LtiRequestsRepo();
			var consumersRepo = new ConsumersRepo();
			var visitsRepo = new VisitsRepo();

			var ltiRequest = ltiRequestsRepo.Find(userId, slide.Id);
			if (ltiRequest == null)
				throw new Exception("LtiRequest for user '" + userId + "' not found");

			var consumerSecret = consumersRepo.Find(ltiRequest.ConsumerKey).Secret;

			var score = visitsRepo.GetScore(slide.Id, userId);

			var uri = new UriBuilder(ltiRequest.LisOutcomeServiceUrl);
			if (uri.Host == "localhost")
			{
				uri.Host = "192.168.33.10";
				uri.Port = 80;
				uri.Scheme = "http";
			}

			var outputScore = score / (double)slide.MaxScore;
			/* Sometimes score is bigger then slide's MaxScore, i.e. in case of manual checking */
			if (score > slide.MaxScore)
				outputScore = 1;
			var result = OutcomesClient.PostScore(uri.ToString(), ltiRequest.ConsumerKey, consumerSecret,
				ltiRequest.LisResultSourcedId, outputScore);

			if (!result.IsValid)
				throw new Exception(uri + "\r\n\r\n" + result.Message);
		}

	}
}