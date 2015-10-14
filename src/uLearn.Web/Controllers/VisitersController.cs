using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using uLearn.Web.DataContexts;

namespace uLearn.Web.Controllers
{
	public class VisitersController : Controller
	{
		private readonly VisitersRepo visitersRepo = new VisitersRepo();
		private readonly CourseManager courseManager = WebCourseManager.Instance;

		[HttpPost]
		[Authorize]
		public async Task<ActionResult> Upload()
		{
			var visits = new StreamReader(Request.InputStream).ReadToEnd();
			var visitsDictionary = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(visits);
			var userId = User.Identity.GetUserId();
			var visiters = new List<Tuple<string, string, string, DateTime>>();
			foreach (var dateTime in visitsDictionary)
			{
				var path = dateTime.Key.Split('/');
				var courseId = path[0];
				var slideIndex = int.Parse(path[1]);
				var slideId = courseManager.GetCourse(path[0]).Slides[slideIndex].Id;
				visiters.Add(Tuple.Create(courseId, slideId, userId, dateTime.Value));
			}
			await visitersRepo.AddVisiters(visiters);
			return null;
		}
	}
}