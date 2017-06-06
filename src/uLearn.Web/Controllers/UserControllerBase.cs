using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using Database.DataContexts;
using Database.Models;
using Kontur.Spam.Client;
using log4net;
using Microsoft.AspNet.Identity;

namespace uLearn.Web.Controllers
{
	public class UserControllerBase : Controller
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(UserControllerBase));

		protected readonly ULearnDb db;
		protected UserManager<ApplicationUser> userManager;
		protected readonly UsersRepo usersRepo;
		protected readonly string secretForHashes;

		protected string spamChannelId;
		protected SpamClient spamClient;
		protected string spamTemplateId;

		protected UserControllerBase(ULearnDb db)
		{
			this.db = db;
			userManager = new ULearnUserManager(db);
			usersRepo = new UsersRepo(db);

			secretForHashes = WebConfigurationManager.AppSettings["ulearn.secretForHashes"] ?? "";

			var spamEndpoint = WebConfigurationManager.AppSettings["ulearn.spam.endpoint"] ?? "";
			var spamLogin = WebConfigurationManager.AppSettings["ulearn.spam.login"] ?? "ulearn";
			var spamPassword = WebConfigurationManager.AppSettings["ulearn.spam.password"] ?? "";
			spamChannelId = WebConfigurationManager.AppSettings["ulearn.spam.channels.emailConfirmations"] ?? "";
			spamTemplateId = WebConfigurationManager.AppSettings["ulearn.spam.templates.withButton"] ?? "";

			try
			{
				spamClient = new SpamClient(new Uri(spamEndpoint), spamLogin, spamPassword);
			}
			catch (Exception e)
			{
				log.Error($"Can\'t initialize Spam.API client to {spamEndpoint}, login {spamLogin}, password {spamPassword.MaskAsSecret()}", e);
				throw;
			}
		}

		protected UserControllerBase() : this(new ULearnDb())
		{
		}

		protected string GetEmailConfirmationSignature(string email)
		{
			return $"{secretForHashes}email={email}{secretForHashes}".CalculateMd5();
		}

		protected async Task<bool> SendConfirmationEmail(ApplicationUser user)
		{
			var confirmationUrl = Url.Action("ConfirmEmail", "Account", new { email = user.Email, signature = GetEmailConfirmationSignature(user.Email) }, "https");
			var subject = "Подтверждение адреса";

			var messageInfo = new MessageSentInfo
			{
				RecipientAddress = user.Email,
				RecipientName = user.VisibleName,
				Subject = subject,
				TemplateId = spamTemplateId,
				Variables = new Dictionary<string, object>
				{
					{ "title", subject },
					{ "content", $"<h2>Привет, {user.VisibleName}!</h2><p>Подтвердите адрес электронной почты, перейдя по ссылке:</p>" },
					{ "text_content", $"Привет, {user.VisibleName}!\nПодтвердите адрес электронной почты, перейдя по ссылке:" },
					{ "button", true },
					{ "button_link", confirmationUrl },
					{ "button_text", "Подтвердить адрес" },
					{ "content_after_button",
						"<p>Подтвердив адрес, вы сможете восстановить доступ к своему аккаунту " +
						"в любой момент, а также получать уведомления о том, что происходит " +
						"в ваших курсах.</p><p>Мы не подпишем вас ни на какую периодическую рассылку, " +
						"а все уведомления можно выключить в вашем профиле.</p><p>" +
						"Если ссылка для подтверждения почты не работает, просто скопируйте адрес " +
						$"и вставьте его в адресную строку браузера: <a href=\"{confirmationUrl}\">{confirmationUrl}</a></p>" }
				}
			};

			try
			{
				await spamClient.SentMessageAsync(spamChannelId, messageInfo);
			}
			catch (Exception e)
			{
				log.Error($"Не могу отправить письмо для подтверждения адреса на {user.Email}", e);
				return false;
			}

			await usersRepo.UpdateLastConfirmationEmailTime(user);
			return true;
		}
	}
}