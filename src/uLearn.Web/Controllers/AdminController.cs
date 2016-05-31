using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using uLearn.Quizes;
using uLearn.Web.DataContexts;
using uLearn.Web.FilterAttributes;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize(MinAccessLevel = CourseRole.CourseAdmin)]
	public class AdminController : Controller
	{
		private readonly CourseManager courseManager;
		private readonly ULearnDb db;
		private readonly UsersRepo usersRepo;
		private readonly CommentsRepo commentsRepo;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly QuizzesRepo quizzesRepo;
		private readonly CoursesRepo coursesRepo;
		private readonly UserQuizzesRepo userQuizzesRepo;

		public AdminController()
		{
			db = new ULearnDb();
			courseManager = WebCourseManager.Instance;
			usersRepo = new UsersRepo(db);
			commentsRepo = new CommentsRepo(db);
			userManager = new ULearnUserManager();
			quizzesRepo = new QuizzesRepo(db);
			coursesRepo = new CoursesRepo(db);
			userQuizzesRepo = new UserQuizzesRepo(db);
		}

		public ActionResult CourseList(string courseCreationLastTry = null)
		{
			var courses = new HashSet<string>(User.GetControllableCoursesId());
			var incorrectChars = new string(CourseManager.GetInvalidCharacters().OrderBy(c => c).Where(c => 32 <= c).ToArray());
			var model = new CourseListViewModel
			{
				Courses = courseManager.GetCourses().Where(course => courses.Contains(course.Id)).Select(course => new CourseViewModel
				{
					Id = course.Id,
					Title = course.Title,
					LastWriteTime = courseManager.GetLastWriteTime(course.Id)
				}).ToList(),
				CourseCreationLastTry = courseCreationLastTry,
				InvalidCharacters = incorrectChars
			};
			return View(model);
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

		public ActionResult DownloadVersion(string courseId, Guid versionId)
		{
			var packageName = courseManager.GetPackageName(courseId);
			return File(courseManager.GetCourseVersionFile(versionId).FullName, "application/zip", packageName);
		}

		private void CreateQuizVersionsForSlides(string courseId, IEnumerable<Slide> slides)
		{
			foreach (var slide in slides.OfType<QuizSlide>())
				quizzesRepo.AddQuizVersionIfNeeded(courseId, slide);
		}

		[HttpPost]
		public async Task<ActionResult> UploadCourse(string courseId, HttpPostedFileBase file)
		{
			if (file == null || file.ContentLength <= 0)
				return RedirectToAction("Packages", new { courseId });

			var fileName = Path.GetFileName(file.FileName);
			if (fileName == null || !fileName.ToLower().EndsWith(".zip"))
				return RedirectToAction("Packages", new { courseId });

			var versionId = Guid.NewGuid();
			
			var destinationFile = courseManager.GetCourseVersionFile(versionId);
			file.SaveAs(destinationFile.FullName);

			/* Load version and put it into LRU-cache */
			courseManager.GetVersion(versionId);
			await coursesRepo.AddCourseVersion(courseId, versionId, User.Identity.GetUserId());

			return RedirectToAction("Diagnostics", new { courseId, versionId });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ULearnAuthorize(Roles = LmsRoles.SysAdmin)]
		public ActionResult CreateCourse(string courseId)
		{
			if (!courseManager.TryCreateCourse(courseId))
				return RedirectToAction("CourseList", new { courseCreationLastTry = courseId });
			return RedirectToAction("Users", new { courseId, onlyPrivileged = true });
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
			var lastUpdate = courseManager.GetLastWriteTime(courseId);
			var courseVersions = coursesRepo.GetCourseVersions(courseId).ToList();
			var publishedVersion = coursesRepo.GetPublishedCourseVersion(courseId);
			return View(model: new PackagesViewModel
			{
				CourseId = courseId,
				HasPackage = hasPackage,
				LastUpdate = lastUpdate,
				Versions = courseVersions,
				PublishedVersion = publishedVersion,
			});
		}

		public ActionResult Comments(string courseId)
		{
			var course = courseManager.GetCourse(courseId);
			var commentsPolicy = commentsRepo.GetCommentsPolicy(courseId);

			var comments = commentsRepo.GetCourseComments(courseId).OrderByDescending(x => x.PublishTime).ToList();
			var commentsLikes = commentsRepo.GetCommentsLikesCounts(comments);
			var commentsLikedByUser = commentsRepo.GetCourseCommentsLikedByUser(courseId, User.Identity.GetUserId());
			var commentsById = comments.ToDictionary(x => x.Id);

			return View(new AdminCommentsViewModel
			{
				CourseId = courseId,
				IsCommentsEnabled = commentsPolicy.IsCommentsEnabled,
				ModerationPolicy = commentsPolicy.ModerationPolicy,
				OnlyInstructorsCanReply = commentsPolicy.OnlyInstructorsCanReply,
				Comments = comments.Select(c => new CommentViewModel
				{
					Comment = c,
					LikesCount = commentsLikes.Get(c.Id, 0),
					IsLikedByUser = commentsLikedByUser.Contains(c.Id),
					Replies = new List<CommentViewModel>(),
					CanEditAndDeleteComment = true,
					CanModerateComment = true,
					IsCommentVisibleForUser = true,
					ShowContextInformation = true,
					ContextSlideTitle = course.GetSlideById(c.SlideId).Title,
					ContextParentComment = c.IsTopLevel() ? null : commentsById[c.ParentCommentId].Text,
				}).ToList()
			});
		}

		public ActionResult ManualChecks(string courseId, string message="")
		{
			var course = courseManager.GetCourse(courseId);
			var checks = userQuizzesRepo.GetQueueForManualChecks(courseId);

			return View(new ManualChecksViewModel
			{
				CourseId = courseId,
				Checks = checks.Select(c => new ManualCheckViewModel
				{
					QuizCheckQueueItem = c,
					ContextSlideTitle = course.GetSlideById(c.SlideId).Title
				}).ToList(),
				Message = message,
			});
		}

		public async Task<ActionResult> Check(int id)
		{
			ManualQuizCheckQueueItem quizCheckQueueItem;
			using (var transaction = db.Database.BeginTransaction())
			{
				quizCheckQueueItem = userQuizzesRepo.GetManualCheckById(id);
				if (!User.HasAccessFor(quizCheckQueueItem.CourseId, CourseRole.Instructor))
					return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
				if (quizCheckQueueItem.IsChecked)
					return RedirectToAction("ManualChecks",
						new
						{
							courseId = quizCheckQueueItem.CourseId,
							message = "already_checked"
						});
				if (quizCheckQueueItem.LockedBy != null && quizCheckQueueItem.LockedUntil >= DateTime.Now)
					return RedirectToAction("ManualChecks",
							new
							{
								courseId = quizCheckQueueItem.CourseId,
								message = "locked"
							});

				await userQuizzesRepo.LockManualCheck(quizCheckQueueItem, User.Identity.GetUserId());
				transaction.Commit();
			}
			return RedirectToRoute("Course.SlideById", new
			{
				CourseId = quizCheckQueueItem.CourseId,
				SlideId = quizCheckQueueItem.SlideId,
				User = quizCheckQueueItem.UserId
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SaveCommentsPolicy(AdminCommentsViewModel model)
		{
			var courseId = model.CourseId;
			var commentsPolicy = new CommentsPolicy
			{
				CourseId = courseId,
				IsCommentsEnabled = model.IsCommentsEnabled,
				ModerationPolicy = model.ModerationPolicy,
				OnlyInstructorsCanReply = model.OnlyInstructorsCanReply
			};
			await commentsRepo.SaveCommentsPolicy(commentsPolicy);
			return RedirectToAction("Comments", new { courseId });
		}

		public ActionResult Users(UserSearchQueryModel queryModel)
		{
			if (string.IsNullOrEmpty(queryModel.CourseId))
				return RedirectToAction("CourseList");
			return View(queryModel);
		}

		[ChildActionOnly]
		public ActionResult UsersPartial(UserSearchQueryModel queryModel)
		{
			var userRoles = usersRepo.FilterUsers(queryModel, userManager);
			var model = GetUserListModel(userRoles, queryModel.CourseId);

			return PartialView("_UserListPartial", model);
		}

		private UserListModel GetUserListModel(IEnumerable<UserRolesInfo> userRoles, string courseId)
		{
			var rolesForUsers = db.UserRoles
				.Where(role => role.CourseId == courseId)
				.GroupBy(role => role.UserId)
				.ToDictionary(
					g => g.Key,
					g => g.Select(role => role.Role).Distinct().ToList()
				);

			var model = new UserListModel
			{
				IsCourseAdmin = true,
				ShowDangerEntities = false,
				Users = new List<UserModel>()
			};

			foreach (var userRolesInfo in userRoles)
			{
				var user = new UserModel(userRolesInfo);

				List<CourseRole> roles;
				if (!rolesForUsers.TryGetValue(userRolesInfo.UserId, out roles))
					roles = new List<CourseRole>();

				user.CoursesAccess = Enum.GetValues(typeof(CourseRole))
					.Cast<CourseRole>()
					.Where(courseRole => courseRole != CourseRole.Student)
					.ToDictionary(
						courseRole => courseRole.ToString(),
						courseRole => (ICoursesAccessListModel)new SingleCourseAccessModel
						{
							HasAccess = roles.Contains(courseRole),
							ToggleUrl = Url.Action("ToggleRole", "Account", new { courseId, userId = user.UserId, role = courseRole })
						});

				model.Users.Add(user);
			}

			return model;
		}

		public ActionResult Diagnostics(string courseId, Guid? versionId)
		{
			if (versionId == null)
			{
				return View(new DiagnosticsModel
				{
					CourseId = courseId,
				});
			}

			var versionIdGuid = (Guid) versionId;

			var course = courseManager.GetCourse(courseId);
			var version = courseManager.GetVersion(versionIdGuid);

			var courseDiff = new CourseDiff(course, version);

			return View(new DiagnosticsModel
			{
				CourseId = courseId,
				IsDiagnosticsForVersion = true,
				VersionId = versionIdGuid,
				CourseDiff = courseDiff,
			});
		}

		[HttpPost]
		public async Task<ActionResult> PublishVersion(string courseId, Guid versionId)
		{
			var versionFile = courseManager.GetCourseVersionFile(versionId);
			var courseFile = courseManager.GetStagingCourseFile(courseId);
			var oldCourse = courseManager.GetCourse(courseId);

			/* First, try to load course from LRU-cache or zip file */
			var version = courseManager.GetVersion(versionId);

			/* Copy version's zip file to course's zip file, overwrite if need */
			versionFile.CopyTo(courseFile.FullName, true);

			/* Replace courseId */
			version.Id = courseId;
			courseManager.UpdateCourse(version);

			CreateQuizVersionsForSlides(courseId, version.Slides);
			await coursesRepo.MarkCourseVersionAsPublished(versionId);

			var courseDiff = new CourseDiff(oldCourse, version);

			return View("Diagnostics", new DiagnosticsModel
			{
				CourseId = courseId,
				IsDiagnosticsForVersion = true,
				IsVersionPublished = true,
				VersionId = versionId,
				CourseDiff = courseDiff,
			});
		}

		[HttpPost]
		public async Task<ActionResult> DeleteVersion(string courseId, Guid versionId)
		{
			/* Remove information from database */
			await coursesRepo.DeleteCourseVersion(courseId, versionId);

			/* Delete zip-archive from file system */
			courseManager.GetCourseVersionFile(versionId).Delete();

			return RedirectToAction("Packages", new { courseId });
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
		public string CourseCreationLastTry { get; set; }
		public string InvalidCharacters { get; set; }
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
		public DateTime LastUpdate { get; set; }
		public List<CourseVersion> Versions { get; set; }
		public CourseVersion PublishedVersion { get; set; }
	}

	public class AdminCommentsViewModel
	{
		public string CourseId { get; set; }
		public bool IsCommentsEnabled { get; set; }
		public CommentModerationPolicy ModerationPolicy { get; set; }
		public bool OnlyInstructorsCanReply { get; set; }
		public List<CommentViewModel> Comments { get; set; }
	}

	public class ManualChecksViewModel
	{
		public string CourseId { get; set; }
		public List<ManualCheckViewModel> Checks { get; set; }
		public string Message { get; set; }
	}

	public class ManualCheckViewModel
	{
		public ManualQuizCheckQueueItem QuizCheckQueueItem { get; set; }

		public string ContextSlideTitle { get; set; }
	}

	public class DiagnosticsModel
	{
		public string CourseId { get; set; }

		public bool IsDiagnosticsForVersion { get; set; }
		public bool IsVersionPublished { get; set; }
		public Guid VersionId { get; set; }
		public CourseDiff CourseDiff { get; set; }
	}
}