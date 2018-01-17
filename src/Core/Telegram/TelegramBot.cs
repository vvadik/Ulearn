using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using uLearn.Configuration;

namespace uLearn.Telegram
{
	public class TelegramBot
	{
		private readonly string token;
		protected string channel;
		protected readonly TelegramBotClient telegramClient;

		protected TelegramBot()
		{
			token = ApplicationConfiguration.Read<UlearnConfiguration>().Telegram.BotToken;
			// token = ConfigurationManager.AppSettings["ulearn.telegram.botToken"];
			if (! string.IsNullOrEmpty(token))
				telegramClient = new TelegramBotClient(token);
		}

		protected bool IsBotEnabled => !string.IsNullOrWhiteSpace(token) && !string.IsNullOrEmpty(channel);
	}
}