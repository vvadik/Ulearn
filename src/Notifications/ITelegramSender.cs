using System;
using System.Threading.Tasks;
using Database.Models;
using uLearn.Extensions;

namespace Notifications
{
	public interface ITelegramSender
	{
		Task SendMessageAsync(long chatId, string html, TelegramButton button = null);
	}

	public class TelegramButton
	{
		public TelegramButton(NotificationButton notificationButton)
		{
			Link = notificationButton.Link;

			/* Telegram doesn't support links to localhost so replace domain */
			Link = Link.Replace("localhost", "replace.to.localhost");

			Link = Link.AddQueryParameter("utm_source", "telegram");

			Text = notificationButton.Text;
		}

		public string Link { get; private set; }
		public string Text { get; private set; }

		public override string ToString()
		{
			return $"TelegramButton{{{Text}: <{Link}>}}";
		}
	}
}
