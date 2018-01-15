using System;
using System.Net;
using System.Threading.Tasks;
using AntiPlagiarism.Web.Database.Models;
using AntiPlagiarism.Web.Database.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;


namespace AntiPlagiarism.Web.Controllers
{
	public abstract class BaseController : Controller
	{
		protected readonly ILogger logger;
		//private readonly IConfiguration configuration;
		
		//protected readonly AntiPlagiarismConfiguration ApplicationConfiguration = new AntiPlagiarismConfiguration();
		
		protected Client client;

		protected BaseController(ILogger logger)
		{
			this.logger = logger;
			//this.configuration = configuration;
			//configuration.GetSection("antiplagiarism").Bind(ApplicationConfiguration);
		}

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			/* TODO (andgein): bad code, losing DI greats */
			var clientsRepo = context.HttpContext.RequestServices.GetService<IClientsRepo>();
			
			var token = context.HttpContext.Request.Query["token"].ToString();
			if (string.IsNullOrEmpty(token))
			{
				context.HttpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;
				context.Result = new JsonResult(new
				{
					status = "error",
					error = "E001",
					message = "Not authenticated request. Pass 'token' parameter to query string.",
				});
				return;
			}
			if (! Guid.TryParse(token, out var tokenGuid))
			{
				context.HttpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;
				context.Result = new JsonResult(new
				{
					status = "error",
					error = "E002",
					message = "Not authenticated request. Pass correct GUID as 'token' parameter to query string.",
				});
				return;
			}
			
			logger.Debug($"Token in request is {token}", token);			
			client = await clientsRepo.FindClientByTokenAsync(tokenGuid);
			if (client == null)
			{
				context.HttpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;
				context.Result = new JsonResult(new
				{
					status = "error",
					error = "E003",
					message = "Not authenticated request. Token is invalid or disabled for a while.",
				});
				return;
			}

			await base.OnActionExecutionAsync(context, next);
		}
	}

	public class AntiPlagiarismConfiguration
	{
		public int SnippetTokensCount { get; set; }
	}
}