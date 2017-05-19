using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using uLearn;

namespace Notifications
{
	public class TelegramSender : ITelegramSender
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(KonturSpamEmailSender));
		private readonly TelegramBotClient bot;

		public TelegramSender()
		{
			var botToken = ConfigurationManager.AppSettings["ulearn.telegram.botToken"];
			try
			{
				bot = new TelegramBotClient(botToken);
			}
			catch (Exception e)
			{
				log.Error($"Can\'t initialize telegram bot with token \"{botToken.MaskAsSecret()}\"", e);
				throw;
			}

			log.Info($"Initialized telegram bot with token \"{botToken.MaskAsSecret()}\"");
		}

		public async Task SendMessageAsync(long chatId, string html)
		{
			html = html.Replace("<br>", "\n").Replace("<br/>", "\n");
			log.Info($"Try to send message to telegram chat {chatId}, html: {html.Replace("\n", @" \\ ")}");
			try
			{
				await bot.SendTextMessageAsync(chatId, html, parseMode: ParseMode.Html);
			}
			catch (Exception e)
			{
				log.Error($"Can\'t send message to telegram chat {chatId}", e);
				throw;
			}
		}
	}
}
