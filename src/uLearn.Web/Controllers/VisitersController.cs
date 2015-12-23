using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using uLearn.Web.DataContexts;
using uLearn.Web.FilterAttributes;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize]
	public class VisitersController : Controller
	{
		private readonly VisitersRepo visitersRepo = new VisitersRepo();
		private readonly CourseManager courseManager = WebCourseManager.Instance;

		[HttpPost]
		public async Task<ActionResult> Upload()
		{
			var visits = new StreamReader(Request.InputStream).ReadToEnd();
			var visitsDictionary = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(visits);
			var userId = User.Identity.GetUserId();
			var visiters = new List<Visiters>();
			var slides = courseManager.GetCourses().SelectMany(course => course.Slides.Select(slide => new { courseId = course.Id, slideId = slide.Id })).ToDictionary(arg => arg.slideId, arg => arg.courseId);
			foreach (var visit in visitsDictionary)
			{
				var slideId = visit.Key;
				string courseId;
				if (!slides.TryGetValue(slideId, out courseId))
					continue;
				visiters.Add(new Visiters
				{
					UserId = userId,
					CourseId = courseId,
					SlideId = slideId,
					Timestamp = visit.Value
				});
			}
			await visitersRepo.AddVisiters(visiters);
			return null;
		}
	}
}