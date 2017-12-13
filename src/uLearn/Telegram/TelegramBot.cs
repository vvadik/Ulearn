using System.Configuration;
using Telegram.Bot;

namespace uLearn.Telegram
{
	public class TelegramBot
	{
		private readonly string token;
		protected string channel;
		protected readonly TelegramBotClient telegramClient;

		protected TelegramBot()
		{
			token = ConfigurationManager.AppSettings["ulearn.telegram.botToken"];
			telegramClient = new TelegramBotClient(token);
		}

		protected bool IsBotEnabled => !string.IsNullOrWhiteSpace(token) && !string.IsNullOrEmpty(channel);
	}
}