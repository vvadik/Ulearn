using System;
using System.Web.Configuration;
using Elmah;
using log4net;
using Telegram.Bot.Types.Enums;

namespace uLearn.Web.Telegram
{
	class ErrorsBot : TelegramBot
	{
		private readonly ILog log = LogManager.GetLogger(typeof(ErrorsBot));

		public ErrorsBot()
		{
			channel = WebConfigurationManager.AppSettings["ulearn.telegram.errors.channel"];
		}

		public void PostToChannel(string errorId, Error error)
		{
			if (!IsBotEnabled)
				return;

			var elmahUrl = "https://ulearn.me/elmah/detail?id=" + errorId;

			var text = $"*Произошла ошибка {EscapeMarkdown(errorId)}*\n" + 
				$"{EscapeMarkdown(error.Exception.Message)}\n\n" + 
				$"Подробности: {EscapeMarkdown(elmahUrl)}";
			try
			{
				telegramClient.SendTextMessageAsync(channel, text, parseMode: ParseMode.Markdown, disableWebPagePreview: true)
					.GetAwaiter()
					.GetResult();
			}
			catch (Exception e)
			{
				log.Error($"Не могу отправить сообщение в телеграм-канал {channel}", e);
			}

		}
	}
}