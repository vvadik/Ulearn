using System;
using System.Linq;
using System.Threading.Tasks;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class RestoreRequestRepo
	{
		private readonly ULearnDb db;

		public RestoreRequestRepo()
			: this(new ULearnDb())
		{

		}

		public RestoreRequestRepo(ULearnDb db)
		{
			this.db = db;
		}

		public async Task<string> CreateRequest(string userId)
		{
			var previous = db.RestoreRequests.FirstOrDefault(r => r.UserId == userId);
			if (previous != null)
			{
				if (DateTime.Now.Subtract(previous.LastTry) < TimeSpan.FromMinutes(5))
					return null;
				db.RestoreRequests.Remove(previous);
			}

			var request = new RestoreRequest
			{
				Id = Guid.NewGuid().ToString(),
				UserId = userId,
				LastTry = DateTime.Now
			};
			db.RestoreRequests.Add(request);
			await db.SaveChangesAsync();

			return request.Id;
		}

		public string FindUserId(string requestId)
		{
			var request = db.RestoreRequests.Find(requestId);
			return request == null ? null : request.UserId;
		}

		public async Task DeleteRequest(string requestId)
		{
			var request = db.RestoreRequests.Find(requestId);
			if (request == null)
				return;
			db.RestoreRequests.Remove(request);
			await db.SaveChangesAsync();
		}

		public bool ContainsRequest(string requestId)
		{
			return db.RestoreRequests.Find(requestId) != null;
		}
	}
}