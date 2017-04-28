using System.IO;
using System.Text;
using System.Threading;
using System.Web.Configuration;
using System.Web.Hosting;
using Microsoft.Owin;
using Owin;
using Telegram.Bot;
using uLearn.Web;
using uLearn.Web.Controllers;

[assembly: OwinStartup(typeof (Startup))]

namespace uLearn.Web
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);
			InitTelegramBot();
		}

		public void InitTelegramBot()
		{
			var botToken = WebConfigurationManager.AppSettings["ulearn.telegram.botToken"];
			if (botToken == null)
				return;

			bool useWebhook;
			bool.TryParse(WebConfigurationManager.AppSettings["ulearn.telegram.useWebhook"], out useWebhook);
			var telegramBot = new TelegramBotClient(botToken);
			if (useWebhook)
			{
				var webhookSecret = WebConfigurationManager.AppSettings["ulearn.telegram.webhook.secret"] ?? "";
				var webhookDomain = WebConfigurationManager.AppSettings["ulearn.telegram.webhook.domain"] ?? "";
				var webhookUrl = $"https://{webhookDomain}/Telegram/Webhook?secret={webhookSecret}";
				telegramBot.SetWebhookAsync(webhookUrl).Wait();
			}
			else
			{
				var telegramController = new TelegramController();
				telegramBot.OnMessage += (sender, e) => telegramController.OnMessage(sender, e);

				new Thread(() => telegramBot.StartReceiving()).Start();
			}

		}
	}
}