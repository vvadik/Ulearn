using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;

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
			var visiters = new List<Visiters>();
			foreach (var dateTime in visitsDictionary)
			{
				var path = dateTime.Key.Split(new []{'/'}, 3);
				var courseId = path[0];
				var slideId = path[2];
				if (courseManager.GetCourse(courseId).GetSlideById(slideId) == null)
					continue;
				visiters.Add(new Visiters
				{
					UserId = userId,
					CourseId = courseId,
					SlideId = slideId,
					Timestamp = dateTime.Value
				});
			}
			await visitersRepo.AddVisiters(visiters);
			return null;
		}
	}
}