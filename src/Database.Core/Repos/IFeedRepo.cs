using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos
{
	public interface IFeedRepo
	{
		Task<DateTime?> GetFeedViewTimestamp(string userId, int transportId);
		Task UpdateFeedViewTimestamp(string userId, int transportId, DateTime timestamp);
		Task AddFeedNotificationTransportIfNeeded(string userId);
		Task<FeedNotificationTransport> GetUsersFeedNotificationTransport(string userId);
		Task<FeedNotificationTransport> GetCommentsFeedNotificationTransport();
		Task<DateTime?> GetLastDeliveryTimestamp(FeedNotificationTransport notificationTransport);
		Task<int> GetNotificationsCount(string userId, DateTime from, params FeedNotificationTransport[] transports);
		Task<List<Notification>> GetNotificationForFeedNotificationDeliveries<TProperty>(string userId, Expression<Func<Notification, TProperty>> includePath, params FeedNotificationTransport[] transports);
	}
}