using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LtiLibrary.Core.Lti1;
using Newtonsoft.Json;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	internal class LtiRequestsRepo
	{
		private readonly ULearnDb db;
		private readonly JsonSerializer serializer;

		public LtiRequestsRepo()
		{
			db = new ULearnDb();
			serializer = new JsonSerializer();
		}

		public async Task Update(string userId, string slideId, string ltiRequestJson)
		{
			var ltiRequestModel = FindElement(userId, slideId);

			if (ltiRequestModel == null)
			{
				ltiRequestModel = new LtiRequestModel
				{
					UserId = userId,
					SlideId = slideId,
					Request = ltiRequestJson
				};
			}
			else
				ltiRequestModel.Request = ltiRequestJson;

			db.LtiRequests.AddOrUpdate(ltiRequestModel);
			await db.SaveChangesAsync();
		}

		public LtiRequest Find(string userId, string slideId)
		{
			var ltiRequestModel = FindElement(userId, slideId);
			if (ltiRequestModel == null)
				return null;

			return serializer.Deserialize<LtiRequest>(new JsonTextReader(new StringReader(ltiRequestModel.Request)));
		}

		private LtiRequestModel FindElement(string userId, string slideId)
		{
			return db.LtiRequests.FirstOrDefault(request => request.UserId == userId && request.SlideId == slideId);
		}
	}
}