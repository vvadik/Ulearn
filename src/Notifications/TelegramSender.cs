using System;
using System.Threading.Tasks;
using Vostok.Logging.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Ulearn.Common.Extensions;
using Ulearn.Core.Configuration;
using Ulearn.Core.Metrics;

namespace Notifications
{
	public class TelegramSender : ITelegramSender
	{
		private readonly ILog log = LogProvider.Get().ForContext(typeof(TelegramSender));

		private readonly MetricSender metricSender;

		private readonly TelegramBotClient bot;

		public TelegramSender()
		{
			var configuration = ApplicationConfiguration.Read<UlearnConfiguration>();
			var botToken = configuration.Telegram.BotToken;
			try
			{
				bot = new TelegramBotClient(botToken);
			}
			catch (Exception e)
			{
				log.Error(e, $"Can\'t initialize telegram bot with token \"{botToken.MaskAsSecret()}\"");
				throw;
			}

			log.Info($"Initialized telegram bot with token \"{botToken.MaskAsSecret()}\"");

			metricSender = new MetricSender(configuration.GraphiteServiceName);
		}

		public async Task SendMessageAsync(long chatId, string html, TelegramButton button = null)
		{
			metricSender.SendCount("send_to_telegram.try");
			metricSender.SendCount($"send_to_telegram.try.to.{chatId}");
			html = PrepareHtmlForTelegram(html);
			log.Info($"Try to send message to telegram chat {chatId}, html: {html.Replace("\n", @" \\ ")}" + (button != null ? $", button: {button}" : ""));

			InlineKeyboardMarkup replyMarkup = null;
			if (button != null)
				replyMarkup = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithUrl(button.Text, button.Link) });

			try
			{
				await bot.SendTextMessageAsync(chatId, html, parseMode: ParseMode.Html, replyMarkup: replyMarkup);
			}
			catch (Exception e)
			{
				metricSender.SendCount("send_to_telegram.fail");
				metricSender.SendCount($"send_to_telegram.fail.to.{chatId}");

				var isBotBlockedByUser = (e as ApiRequestException)?.Message.Contains("bot was blocked by the user") ?? false;
				if (isBotBlockedByUser)
				{
					metricSender.SendCount("send_to_telegram.fail.blocked_by_user");
					metricSender.SendCount($"send_to_telegram.fail.blocked_by_user.to.{chatId}");
					log.Warn(e, $"Can\'t send message to telegram chat {chatId}");
				}
				else
					log.Error(e, $"Can\'t send message to telegram chat {chatId}");

				throw;
			}

			metricSender.SendCount("send_to_telegram.success");
			metricSender.SendCount($"send_to_telegram.success.to.{chatId}");
		}

		private static string PrepareHtmlForTelegram(string html)
		{
			html = html.Replace("<br>", "\n").Replace("<br/>", "\n").Replace("<br />", "\n");

			/* https://core.telegram.org/bots/api#html-style
			 * All numerical HTML entities are supported.
			 * The API currently supports only the following named HTML entities: &lt;, &gt;, &amp; and &quot;. */
			html = html.Replace("&apos;", "'");

			return html;
		}
	}
}