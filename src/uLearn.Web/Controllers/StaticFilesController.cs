using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ulearn.Common.Api;
using Ulearn.Common.Extensions;
using Ulearn.Core.Configuration;
using Ulearn.Core.Courses;
using Web.Api.Client;

namespace uLearn.Web.Controllers
{
	public class StaticFilesController : Controller
	{
		private static readonly UlearnConfiguration config = ApplicationConfiguration.Read<UlearnConfiguration>();

		public async Task<ActionResult> CourseFile(string courseId, string path)
		{
			if (path.Contains(".."))
				return HttpNotFound();
			var extension = Path.GetExtension(path);
			var mimeType = CourseStaticFilesHelper.AllowedExtensions.GetOrDefault(extension);
			if (mimeType == null)
				return HttpNotFound();
			IWebApiClient webApiClient = new WebApiClient(new ApiClientSettings(config.BaseUrlApi));
			var response = await webApiClient.GetCourseStaticFile(courseId, path);
			if (response == null)
				return HttpNotFound();
			if (response.HasStream)
				return new FileStreamResult(response.Stream, mimeType);
			if (response.HasContent)
				return new FileContentResult(response.Content.ToArray(), mimeType);
			return HttpNotFound();
		}
	}
}