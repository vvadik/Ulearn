using System;
using System.Configuration;
using System.Threading.Tasks;
using Elmah;
using log4net;
using Telegram.Bot.Types.Enums;
using uLearn.Extensions;

namespace uLearn.Telegram
{
	public class ErrorsBot : TelegramBot
	{
		private readonly ILog log = LogManager.GetLogger(typeof(ErrorsBot));

		public ErrorsBot()
		{
			channel = ConfigurationManager.AppSettings["ulearn.telegram.errors.channel"];
		}

		public async Task PostToChannelAsync(string message, ParseMode parseMode = ParseMode.Default)
		{
			if (!IsBotEnabled)
				return;

			log.Info($"Отправляю в телеграм-канал {channel} сообщение об ошибке:\n{message}");
			try
			{
				await telegramClient.SendTextMessageAsync(channel, message, parseMode: parseMode, disableWebPagePreview: true).ConfigureAwait(false);
			}
			catch (Exception e)
			{
				log.Error($"Не могу отправить сообщение в телеграм-канал {channel}", e);
			}
		}

		public void PostToChannel(string message, ParseMode parseMode = ParseMode.Default)
		{
			PostToChannelAsync(message, parseMode).Wait();
		}

		public void PostToChannel(string errorId, Error error)
		{
			if (!IsBotEnabled)
				return;

			var elmahUrl = "https://ulearn.me/elmah/detail?id=" + errorId;

			var text = $"*Произошла ошибка {errorId.EscapeMarkdown()}*\n" +
						$"{error.Exception.Message.EscapeMarkdown()}\n\n" +
						$"Подробности: {elmahUrl.EscapeMarkdown()}";

			PostToChannel(text, ParseMode.Markdown);
		}
	}
}