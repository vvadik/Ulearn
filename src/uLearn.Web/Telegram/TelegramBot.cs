using System.Text.RegularExpressions;
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

		protected static string EscapeMarkdown(string text)
		{
			return Regex.Replace(text, @"([\[\]|\*_`])", @"\$1");
		}

		protected static string MakeNestedQuotes(string text)
		{
			text = Regex.Replace(text, "(\\s|^)[\"«]", @"$1„");
			return Regex.Replace(text, "[\"»]", @"“");
		}

		protected bool IsBotEnabled => ! string.IsNullOrWhiteSpace(token) && ! string.IsNullOrEmpty(channel);
	}
}