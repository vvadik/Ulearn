using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using uLearn.Web.DataContexts;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	[Authorize(Roles = LmsRoles.Instructor)]
	public class UnitController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly ULearnDb db;

		public UnitController()
		{
			db = new ULearnDb();
			courseManager = CourseManager.AllCourses;
		}

		public ActionResult CourseList()
		{
			return View(courseManager.Courses);
		}

		public ActionResult List(string courseId)
		{
			Course course = courseManager.GetCourse(courseId);
			List<UnitAppearance> appearances = db.Units.Where(u => u.CourseId == course.Id).ToList();
			List<Tuple<string, UnitAppearance>> unitAppearances =
				course.Slides
					.Select(s => s.Info.UnitName)
					.Distinct()
					.Select(unitName => Tuple.Create(unitName, appearances.FirstOrDefault(a => a.UnitName == unitName)))
					.ToList();
			return View(new UnitsListViewModel(course.Id, course.Title, unitAppearances, DateTime.Now));
		}

		[HttpPost]
		public async Task<RedirectToRouteResult> SetPublishTime(string courseId, string unitName, string publishTime)
		{

			var unitAppearance = await db.Units.FirstOrDefaultAsync(u => u.CourseId == courseId && u.UnitName == unitName);
			if (unitAppearance == null)
			{
				unitAppearance = new UnitAppearance
				{
					CourseId = courseId,
					UnitName = unitName,
				};
				db.Units.Add(unitAppearance);
			}
			unitAppearance.UserName = User.Identity.Name;
			unitAppearance.UserId = User.Identity.GetUserId();
			unitAppearance.PublishTime = DateTime.Parse(publishTime);
			await db.SaveChangesAsync();
			return RedirectToAction("List", new { courseId });
		}

		[HttpPost]
		public async Task<RedirectToRouteResult> RemovePublishTime(string courseId, string unitName)
		{
			var unitAppearance = await db.Units.FirstOrDefaultAsync(u => u.CourseId == courseId && u.UnitName == unitName);
			if (unitAppearance != null)
			{
				db.Units.Remove(unitAppearance);
				await db.SaveChangesAsync();
			}
			return RedirectToAction("List", new { courseId });
		}
	}

	public class UnitsListViewModel
	{
		public string CourseId;
		public string CourseTitle;
		public DateTime CurrentDateTime;
		public List<Tuple<string, UnitAppearance>> Units;

		public UnitsListViewModel(string courseId, string courseTitle, List<Tuple<string, UnitAppearance>> units,
			DateTime currentDateTime)
		{
			CourseId = courseId;
			CourseTitle = courseTitle;
			Units = units;
			CurrentDateTime = currentDateTime;
		}
	}
}