using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using uLearn.Web.DataContexts;
using uLearn.Web.FilterAttributes;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize(MinAccessLevel = CourseRoles.CourseAdmin)]
	public class UnitController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly ULearnDb db;

		public UnitController()
		{
			db = new ULearnDb();
			courseManager = WebCourseManager.Instance;
		}

		public ActionResult CourseList()
		{
			var courses = new HashSet<string>(User.GetControllableCoursesId());
			var model = new CourseListViewModel
			{
				Courses = courseManager.GetCourses().Where(course => courses.Contains(course.Id)).Select(course => new CourseViewModel
				{
					Id = course.Id,
					Title = course.Title,
					LastWriteTime = courseManager.GetLastWriteTime(course)
				}).ToList(), 
			};
			return View(model);
		}
		
		[HttpPost]
		public ActionResult ReloadCourse(string courseId, string returnUrl = null)
		{
			courseManager.ReloadCourse(courseId);
			if (returnUrl != null) return Redirect(returnUrl);
			return RedirectToAction("CourseList", new { courseId });
		}

		public ActionResult SpellingErrors(string courseId)
		{
			var course = courseManager.GetCourse(courseId);
			return PartialView(course.SpellCheck());
		}

		public ActionResult List(string courseId)
		{
			var course = courseManager.GetCourse(courseId);
			var appearances = db.Units.Where(u => u.CourseId == course.Id).ToList();
			var unitAppearances =
				course.Slides
					.Select(s => s.Info.UnitName)
					.Distinct()
					.Select(unitName => Tuple.Create(unitName, appearances.FirstOrDefault(a => a.UnitName.RemoveBom() == unitName)))
					.ToList();
			return View(new UnitsListViewModel(course.Id, course.Title, unitAppearances, DateTime.Now));
		}

		[HttpPost]
		public async Task<RedirectToRouteResult> SetPublishTime(string courseId, string unitName, string publishTime)
		{

			var oldInfo = await db.Units.Where(u => u.CourseId == courseId && u.UnitName == unitName).ToListAsync();
			db.Units.RemoveRange(oldInfo);
			var unitAppearance = new UnitAppearance
			{
				CourseId = courseId,
				UnitName = unitName,
				UserName = User.Identity.Name,
				PublishTime = DateTime.Parse(publishTime),
			};
			db.Units.Add(unitAppearance);
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

		public ActionResult DownloadPackage(string courseId)
		{
			var packageName = courseManager.GetPackageName(courseId);
			return File(courseManager.GetStagingCoursePath(courseId), "application/zip", packageName);
		}

		[HttpPost]
		public ActionResult UploadCourse(string courseId, HttpPostedFileBase file)
		{
			if (file == null || file.ContentLength <= 0)
				return RedirectToAction("List", new { courseId });

			var fileName = Path.GetFileName(file.FileName);
			if (fileName == null || !fileName.ToLower().EndsWith(".zip"))
				return RedirectToAction("List", new { courseId });

			var packageName = courseManager.GetPackageName(courseId);
			var destinationFile = courseManager.StagedDirectory.GetFile(packageName);
			file.SaveAs(destinationFile.FullName);
			courseManager.ReloadCourse(courseId);
			return RedirectToAction("List", new { courseId });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ULearnAuthorize(Roles = LmsRoles.SysAdmin)]
		public ActionResult CreateCourse(string courseId)
		{
			courseManager.CreateCourse(courseId);
			return RedirectToAction("List", new { courseId });
		}

		public ActionResult ManageMenu(string courseId)
		{
			var course = courseManager.GetCourse(courseId);
			return PartialView(new ManageMenuViewModel
			{
				CourseId = courseId,
				Title = course.Title
			});
		}

		public ActionResult Packages(string courseId)
		{
			var hasPackage = courseManager.HasPackageFor(courseId);
			return View(model: new PackagesViewModel
			{
				CourseId = courseId,
				HasPackage = hasPackage
			});
		}

		public ActionResult Users(string courseId, bool onlyPrivileged = false, string namePrefix = null, string role = null, CourseRoles? courseRole = null)
		{
			return View(new UsersListViewModel
			{
				CourseId = courseId,
				CourseRole = courseRole,
				NamePrefix = namePrefix,
				OnlyPrivileged = onlyPrivileged,
				Role = role
			});
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

	public class CourseListViewModel
	{
		public List<CourseViewModel> Courses;
	}

	public class CourseViewModel
	{
		public string Title { get; set; }
		public string Id { get; set; }
		public DateTime LastWriteTime { get; set; }
	}

	public class ManageMenuViewModel
	{
		public string CourseId { get; set; }
		public string Title { get; set; }
	}

	public class PackagesViewModel
	{
		public string CourseId { get; set; }
		public bool HasPackage { get; set; }
	}

	public class UsersListViewModel
	{
		public string CourseId { get; set; }
		public string Role { get; set; }
		public string NamePrefix { get; set; }
		public CourseRoles? CourseRole { get; set; }
		public bool OnlyPrivileged { get; set; }
	}
}