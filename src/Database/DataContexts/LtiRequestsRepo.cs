using System;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using EntityFramework.Functions;
using LtiLibrary.Core.Lti1;
using Newtonsoft.Json;
using uLearn;

namespace Database.DataContexts
{
	public class LtiRequestsRepo
	{
		private readonly ULearnDb db;
		private readonly JsonSerializer serializer;

		public LtiRequestsRepo(ULearnDb db)
		{
			this.db = db;
			serializer = new JsonSerializer();
		}

		public async Task Update(string userId, Guid slideId, string ltiRequestJson)
		{
			await FuncUtils.TrySeveralTimesAsync(() => TryUpdate(userId, slideId, ltiRequestJson), 3);
		}

		private async Task TryUpdate(string userId, Guid slideId, string ltiRequestJson)
		{
			var ltiRequestModel = FindElement(userId, slideId);

			if (ltiRequestModel == null)
			{
				ltiRequestModel = new LtiSlideRequest
				{
					UserId = userId,
					SlideId = slideId,
					Request = ltiRequestJson
				};
			}
			else
				ltiRequestModel.Request = ltiRequestJson;

			db.LtiRequests.AddOrUpdate(ltiRequestModel);
			await db.ObjectContext().SaveChangesAsync(SaveOptions.DetectChangesBeforeSave);
			db.ObjectContext().AcceptAllChanges();
		}

		public LtiRequest Find(string userId, Guid slideId)
		{
			var ltiRequestModel = FindElement(userId, slideId);
			if (ltiRequestModel == null)
				return null;

			return serializer.Deserialize<LtiRequest>(new JsonTextReader(new StringReader(ltiRequestModel.Request)));
		}

		private LtiSlideRequest FindElement(string userId, Guid slideId)
		{
			return db.LtiRequests.FirstOrDefault(request => request.UserId == userId && request.SlideId == slideId);
		}
	}
}