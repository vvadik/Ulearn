using System.Configuration;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using uLearn.Telegram;

namespace GiftsGranter
{
	public class GiftsTelegramBot : TelegramBot
	{
		public GiftsTelegramBot()
		{
			channel = ConfigurationManager.AppSettings["ulearn.telegram.gifts.channel"];
		}

		public async Task PostToChannelAsync(string message, ParseMode parseMode = ParseMode.Default)
		{
			if (!IsBotEnabled)
				return;
			await telegramClient.SendTextMessageAsync(channel, message, parseMode: parseMode, disableWebPagePreview: true).ConfigureAwait(false);
		}

		public void PostToChannel(string message, ParseMode parseMode = ParseMode.Default)
		{
			PostToChannelAsync(message, parseMode).Wait();
		}
	}
}