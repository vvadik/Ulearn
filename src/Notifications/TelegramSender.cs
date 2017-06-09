using System;
using System.Configuration;
using System.Threading.Tasks;
using log4net;
using Metrics;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using uLearn;

namespace Notifications
{
	public class TelegramSender : ITelegramSender
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(TelegramSender));

		private readonly GraphiteMetricSender metricSender;

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

			metricSender = new GraphiteMetricSender("notifications");
		}

		public async Task SendMessageAsync(long chatId, string html, TelegramButton button = null)
		{
			metricSender.SendCount("send_to_telegram.try");
			html = html.Replace("<br>", "\n").Replace("<br/>", "\n");
			log.Info($"Try to send message to telegram chat {chatId}, html: {html.Replace("\n", @" \\ ")}" + (button != null ? $", button: {button}" : ""));

			InlineKeyboardMarkup replyMarkup = null;
			if (button != null)
				replyMarkup = new InlineKeyboardMarkup(new[] { new InlineKeyboardButton { Url = button.Link, Text = button.Text } });

			try
			{
				await bot.SendTextMessageAsync(chatId, html, parseMode: ParseMode.Html, replyMarkup: replyMarkup);
			}
			catch (Exception e)
			{
				log.Error($"Can\'t send message to telegram chat {chatId}", e);
				throw;
			}
			metricSender.SendCount("send_to_telegram.success");
		}
	}
}
