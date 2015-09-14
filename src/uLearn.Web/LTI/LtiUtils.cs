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
			var visitersRepo = new VisitersRepo();

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

	}
}