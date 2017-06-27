using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Models;

namespace Database.DataContexts
{
	public class FeedRepo
	{
		private readonly ULearnDb db;
		private readonly NotificationsRepo notificationsRepo;

		public FeedRepo(ULearnDb db)
		{
			this.db = db;
			notificationsRepo = new NotificationsRepo(db);
		}

		public DateTime GetFeedUpdateTimestamp(string userId)
		{
			var updateTimestamp = db.FeedUpdateTimestamps.Where(t => t.UserId == userId).OrderByDescending(t => t.Timestamp).FirstOrDefault();
			return updateTimestamp?.Timestamp ?? DateTime.MinValue;
		}

		public void UpdateFeedUpdateTimestamp(string userId, DateTime timestamp)
		{
			db.FeedUpdateTimestamps.AddOrUpdate(t => t.UserId == userId, new FeedUpdateTimestamp
			{
				UserId = userId,
				Timestamp = timestamp
			});
		}

		public async Task AddFeedNotificationTransportIfNeeded(string userId)
		{
			if (notificationsRepo.FindUsersNotificationTransport<FeedNotificationTransport>(userId, includeDisabled: true) != null)
				return;

			await notificationsRepo.AddNotificationTransport(new FeedNotificationTransport
			{
				UserId = userId,
				IsEnabled = true,
			});
		}
	}
}
