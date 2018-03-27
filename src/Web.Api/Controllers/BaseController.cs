using Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Web.Api.Controllers
{
	public class BaseController : Controller
	{
		protected readonly ILogger logger;
		protected readonly WebCourseManager courseManager;

		public BaseController(ILogger logger, WebCourseManager courseManager)
		{
			this.logger = logger;
			this.courseManager = courseManager;
		}
	}
}