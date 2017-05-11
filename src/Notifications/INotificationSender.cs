using System.Threading.Tasks;
using Database.Models;

namespace Notifications
{
	public interface INotificationSender
	{
		Task SendAsync(NotificationDelivery notificationDelivery);
	}
}