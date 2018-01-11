using AntiPlagiarism.Web.Database;
using AntiPlagiarism.Web.Database.Repos;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Vostok.Airlock;
using Vostok.Logging;
using Vostok.Tracing;

namespace AntiPlagiarism.Web.Controllers
{
    [Route("/Home")]
    public class HomeController : BaseController
    {
		public HomeController(IClientsRepo cliensRepo, ILogger logger)
			: base(logger)
		{
			
		}
		
        [HttpGet("{*url}")]
        public object Echo()
        {
            return Json(new
            {
                url = Request.GetUri(),
                traceUrl = $"http://localhost:6301/{TraceContext.Current.TraceId}",
                traceId = TraceContext.Current.TraceId
            });
        }
	}
}
