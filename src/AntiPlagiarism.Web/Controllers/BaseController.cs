using System;
using System.Net;
using AntiPlagiarism.Web.Database.Models;
using AntiPlagiarism.Web.Database.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Serilog;


namespace AntiPlagiarism.Web.Controllers
{
	public abstract class BaseController : Controller
	{
		protected readonly ILogger logger;
		protected Client client;

		protected BaseController(ILogger logger)
		{
			this.logger = logger;
		}
		
		public override void OnActionExecuting(ActionExecutingContext context)
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
			logger.Information($"Token = {token}");
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
			
			client = clientsRepo.FindClientByTokenAsync(tokenGuid).Result;
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

			base.OnActionExecuting(context);
		}

	}
}