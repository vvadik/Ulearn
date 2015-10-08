using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using Exceptions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SendGrid;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class RestorePasswordController : Controller
	{
		private readonly RestoreRequestRepo requestRepo = new RestoreRequestRepo();
		private readonly UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ULearnDb()));

		public ActionResult Index()
		{
			return View(new RestorePasswordModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Index(string username)
		{
			var user = await userManager.FindByNameAsync(username);
			var answer = new RestorePasswordModel
			{
				UserName = username,
				HasError = true
			};

			if (user == null)
			{
				answer.Message = "Пользователь с таким именем не найден";
				return View(answer);
			}
			if (string.IsNullOrWhiteSpace(user.Email))
			{
				answer.Message = "У данного пользователя не указан email";
				return View(answer);
			}

			var requestId = await requestRepo.CreateRequest(user.Id);

			if (requestId == null)
			{
				answer.Message = "Слишком частые запросы. Попробуйте ещё раз через несколько минут";
				return View(answer);
			}

			await SendRestorePasswordEmail(requestId, user);

			answer.HasError = false;
			answer.Message = "Письмо с инструкцией по восстановлению отправлено на Ваш email";
			return View(answer);
		}

		private async Task SendRestorePasswordEmail(string requestId, ApplicationUser user)
		{
			var url = Url.Action("SetNewPassword", "RestorePassword", new { requestId }, "https");

			var message = new SendGridMessage();
			message.AddTo(user.Email);
			message.From = new MailAddress("noreply@ulearn.azurewebsites.net", "Добрый робот uLearn");
			message.Subject = "Восстановление пароля uLearn";
			message.Html = "Чтобы изменить пароль к аккаунту " + user.UserName + ", перейдите по ссылке: <a href=\"" + url + "\">" + url + "</a>";

			var login = ConfigurationManager.AppSettings["SendGrid.Login"];
			var password = ConfigurationManager.AppSettings["SendGrid.Password"];
			var credentials = new NetworkCredential(login, password);

			var transport = new SendGrid.Web(credentials);

			try
			{
				await transport.DeliverAsync(message);
			}
			catch (InvalidApiRequestException ex)
			{
				throw new Exception(ex.Message + ":\n\n" + string.Join("\n", ex.Errors), ex);
			}
		}

		public ActionResult SetNewPassword(string requestId)
		{
			if (!requestRepo.ContainsRequest(requestId))
				return View(new SetNewPasswordModel
				{
					Errors = new[] { "Запрос не найден" }
				});

			return View(new SetNewPasswordModel
			{
				RequestId = requestId,
				Errors = new string[0]
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SetNewPassword(SetNewPasswordModel model)
		{
			var answer = new SetNewPasswordModel
			{
				RequestId = model.RequestId
			};
			if (!ModelState.IsValid)
			{
				answer.Errors = ModelState.Values.SelectMany(state => state.Errors.Select(error => error.ErrorMessage)).ToArray();
				return View(answer);
			}

			var userId = requestRepo.FindUserId(model.RequestId);
			if (userId == null)
			{
				answer.Errors = new[] { "Запрос не найден" };
				answer.RequestId = null;
				return View(answer);
			}

			var result = await userManager.RemovePasswordAsync(userId);
			if (!result.Succeeded)
			{
				answer.Errors = result.Errors.ToArray();
				return View(answer);
			}

			result = await userManager.AddPasswordAsync(userId, model.NewPassword);
			if (!result.Succeeded)
			{
				answer.Errors = result.Errors.ToArray();
				return View(answer);
			}

			await requestRepo.DeleteRequest(model.RequestId);

			return RedirectToAction("Index", "Home");
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			if (filterContext.Exception is HttpAntiForgeryException)
			{
				filterContext.ExceptionHandled = true;
				filterContext.Result = RedirectToAction("Index", "Login");
			}
			base.OnException(filterContext);
		}
	}
}