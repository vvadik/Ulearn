using System.Web.Configuration;
using Telegram.Bot;

namespace uLearn.Web.Telegram
{
	public class TelegramBot
	{
		private readonly string token;
		protected string channel;
		protected TelegramBotClient telegramClient;

		public TelegramBot()
		{
			token = WebConfigurationManager.AppSettings["ulearn.telegram.token"];
			telegramClient = new TelegramBotClient(token);
		}
		
		protected bool IsBotEnabled => ! string.IsNullOrWhiteSpace(token) && ! string.IsNullOrEmpty(channel);
	}
}