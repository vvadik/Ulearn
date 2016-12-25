using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using log4net;
using LtiLibrary.Owin.Security.Lti.Provider;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;

namespace uLearn.Web.LTI
{
	public static class SecurityHandler
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(SecurityHandler));

		/// <summary>
		/// Invoked after the LTI request has been authenticated so the application can sign in the application user.
		/// </summary>
		/// <param name="context">Contains information about the login session as well as the LTI request.</param>
		/// <param name="claims">Optional set of claims to add to the identity.</param>
		/// <returns>A <see cref="Task"/> representing the completed operation.</returns>
		public static async Task OnAuthenticated(LtiAuthenticatedContext context, IEnumerable<Claim> claims = null)
		{
			log.Info($"Lti вызвал OnAuthenticated: {context.Request.Uri}");

			ClaimsIdentity identity;
			if (!IsAuthenticated(context.OwinContext))
			{
				// Find existing pairing between LTI user and application user
				var userManager = new ULearnUserManager();
				var loginProvider = string.Join(":", context.Options.AuthenticationType, context.LtiRequest.ConsumerKey);
				var providerKey = context.LtiRequest.UserId;
				var login = new UserLoginInfo(loginProvider, providerKey);
				var user = await userManager.FindAsync(login);
				log.Info($"Ищу пользователя: провайдер {loginProvider}, идентификатор {providerKey}");
				if (user == null)
				{
					log.Info("Не нашёл пользователя");
					var usernameContext = new LtiGenerateUserNameContext(context.OwinContext, context.LtiRequest);
					log.Info("Генерирую имя пользователя для LTI-пользователя");
					await context.Options.Provider.GenerateUserName(usernameContext);
					if (string.IsNullOrEmpty(usernameContext.UserName))
					{
						throw new Exception("Can't generate username");
					}
					log.Info($"Сгенерировал: {usernameContext.UserName}, ищу пользователя по этому имени");
					user = await userManager.FindByNameAsync(usernameContext.UserName);
					if (user == null)
					{
						log.Info("Не нашёл пользователя с таким именем, создаю нового");
						user = new ApplicationUser { UserName = usernameContext.UserName };
						var result = await userManager.CreateAsync(user);
						if (!result.Succeeded)
						{
							var errors = string.Join("\n\n", result.Errors);
							throw new Exception("Can't create user: " + errors);
						}
					}
					log.Info($"Добавляю LTI-логин {login} к пользователю {user.VisibleName} (Id = {user.Id})");
					// Save the pairing between LTI user and application user
					await userManager.AddLoginAsync(user.Id, login);
				}

				log.Info($"Подготавливаю identity для пользователя {user.VisibleName} (Id = {user.Id})");
				// Create the application identity, add the LTI request as a claim, and sign in
				identity = await userManager.CreateIdentityAsync(user, context.Options.SignInAsAuthenticationType);
			}
			else
			{
				log.Info($"Пришёл LTI-запрос на аутенфикацию, но пользователь уже аутенфицирован на ulearn: {context.OwinContext.Authentication.User.Identity}");
				identity = (ClaimsIdentity)context.OwinContext.Authentication.User.Identity;
			}

			var json = JsonConvert.SerializeObject(context.LtiRequest, Formatting.None,
				new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

			log.Info($"LTI-запрос: {json}");
			
			var claimsToRemove = identity.Claims.Where(c => c.Type.Equals("LtiRequest"));
			foreach (var claim in claimsToRemove)
			{
				log.Info($"Требование в LTI-запросе: {claim}");
				identity.RemoveClaim(claim);
			}

			identity.AddClaim(new Claim(context.Options.ClaimType, json, ClaimValueTypes.String, context.Options.AuthenticationType));
			if (claims != null)
			{
				foreach (var claim in claims)
				{
					identity.AddClaim(claim);
				}
			}
			log.Info($"Аутенфицирую identity: {identity}");
			context.OwinContext.Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

			// Redirect to original URL so the new identity takes affect
			context.RedirectUrl = context.LtiRequest.Url.ToString();
		}

		/// <summary>
		/// Generate a valid application username using information from an LTI request. The default
		/// ASP.NET application using Microsoft Identity uses an email address as the username. This
		/// code will generate an "anonymous" email address if one is not supplied in the LTI request.
		/// </summary>
		/// <param name="context">Contains information about the login session as well as the LTI request.</param>
		/// <returns>A <see cref="Task"/> representing the completed operation.</returns>
		public static Task OnGenerateUserName(LtiGenerateUserNameContext context)
		{
			if (string.IsNullOrEmpty(context.LtiRequest.LisPersonEmailPrimary))
			{
				var username = context.LtiRequest.UserId;
				Uri url;
				if (string.IsNullOrEmpty(context.LtiRequest.ToolConsumerInstanceUrl)
					|| !Uri.TryCreate(context.LtiRequest.ToolConsumerInstanceUrl, UriKind.Absolute, out url))
				{
					context.UserName = string.Concat(username, "@", context.LtiRequest.ConsumerKey);
				}
				else
				{
					context.UserName = string.Concat(username, "@", url.Host);
				}
			}
			else
			{
				context.UserName = context.LtiRequest.LisPersonEmailPrimary;
			}

			return Task.FromResult<object>(null);
		}

		private static bool IsAuthenticated(IOwinContext context)
		{
			var auth = context.Authentication;
			return auth.User?.Identity != null && auth.User.Identity.IsAuthenticated;
		}
	}
}