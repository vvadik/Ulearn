using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Database;
using Database.DataContexts;
using Database.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using uLearn.Web.FilterAttributes;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize]
	public class VisitsController : Controller
	{
		private readonly VisitsRepo visitsRepo = new VisitsRepo(new ULearnDb());
		private readonly CourseManager courseManager = WebCourseManager.Instance;

		[HttpPost]
		public async Task<ActionResult> Upload()
		{
			var visitsAsText = new StreamReader(Request.InputStream).ReadToEnd();
			var visitsDictionary = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(visitsAsText);
			var userId = User.Identity.GetUserId();
			var visits = new List<Visit>();
			var slides = courseManager.GetCourses().SelectMany(course => course.Slides.Select(slide => new { courseId = course.Id, slideId = slide.Id })).ToDictionary(arg => arg.slideId, arg => arg.courseId);
			foreach (var visit in visitsDictionary)
			{
				var slideId = Guid.Parse(visit.Key);
				string courseId;
				if (!slides.TryGetValue(slideId, out courseId))
					continue;
				visits.Add(new Visit
				{
					UserId = userId,
					CourseId = courseId,
					SlideId = slideId,
					Timestamp = visit.Value
				});
			}
			await visitsRepo.AddVisits(visits);
			return null;
		}
	}
}