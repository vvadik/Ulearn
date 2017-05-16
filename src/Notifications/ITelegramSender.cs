using System.Threading.Tasks;

namespace Notifications
{
	public interface ITelegramSender
	{
		Task SendMessageAsync(long chatId, string html);
	}
}
