using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using uLearn.Quizes;
using uLearn.Web.DataContexts;
using uLearn.Web.FilterAttributes;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
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
		private readonly GroupsRepo groupsRepo;

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
			groupsRepo = new GroupsRepo(db);
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

		[ULearnAuthorize(MinAccessLevel = CourseRole.CourseAdmin)]
		public ActionResult SpellingErrors(string courseId)
		{
			var course = courseManager.GetCourse(courseId);
			return PartialView(course.SpellCheck());
		}

		[ULearnAuthorize(MinAccessLevel = CourseRole.CourseAdmin)]
		public ActionResult Units(string courseId)
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
		[ULearnAuthorize(MinAccessLevel = CourseRole.CourseAdmin)]
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
			return RedirectToAction("Units", new { courseId });
		}

		[HttpPost]
		[ULearnAuthorize(MinAccessLevel = CourseRole.CourseAdmin)]
		public async Task<RedirectToRouteResult> RemovePublishTime(string courseId, string unitName)
		{
			var unitAppearance = await db.Units.FirstOrDefaultAsync(u => u.CourseId == courseId && u.UnitName == unitName);
			if (unitAppearance != null)
			{
				db.Units.Remove(unitAppearance);
				await db.SaveChangesAsync();
			}
			return RedirectToAction("Units", new { courseId });
		}

		[ULearnAuthorize(MinAccessLevel = CourseRole.CourseAdmin)]
		public ActionResult DownloadPackage(string courseId)
		{
			var packageName = courseManager.GetPackageName(courseId);
			return File(courseManager.GetStagingCoursePath(courseId), "application/zip", packageName);
		}

		[ULearnAuthorize(MinAccessLevel = CourseRole.CourseAdmin)]
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
		[ULearnAuthorize(MinAccessLevel = CourseRole.CourseAdmin)]
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

		[ULearnAuthorize(MinAccessLevel = CourseRole.CourseAdmin)]
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
				Comments = (from c in comments
							let slide = course.FindSlideById(c.SlideId)
							where slide != null
							select
							new CommentViewModel
							{
								Comment = c,
								LikesCount = commentsLikes.Get(c.Id, 0),
								IsLikedByUser = commentsLikedByUser.Contains(c.Id),
								Replies = new List<CommentViewModel>(),
								CanEditAndDeleteComment = true,
								CanModerateComment = true,
								IsCommentVisibleForUser = true,
								ShowContextInformation = true,
								ContextSlideTitle = slide.Title,
								ContextParentComment = c.IsTopLevel() ? null : commentsById[c.ParentCommentId].Text,
							}).ToList()
			});
		}

		public ActionResult ManualQuizChecksQueue(string courseId, string message="")
		{
			var course = courseManager.GetCourse(courseId);
			var checks = userQuizzesRepo.GetManualQuizCheckQueue(courseId).ToList();

			if (!checks.Any() && ! string.IsNullOrEmpty(message))
				return RedirectToAction("ManualQuizChecksQueue", new { courseId });

			return View(new ManualQuizCheckQueueViewModel
			{
				CourseId = courseId,
				Checks = checks.Select(c => new ManualQuizCheckQueueItemViewModel
				{
					QuizCheckQueueItem = c,
					ContextSlideTitle = course.GetSlideById(c.SlideId).Title
				}).ToList(),
				Message = message,
			});
		}

		private async Task<ActionResult> InternalCheckQuiz(int queueItemId, bool ignoreLock = false)
		{
			ManualQuizCheckQueueItem quizCheckQueueItem;
			using (var transaction = db.Database.BeginTransaction())
			{
				quizCheckQueueItem = userQuizzesRepo.GetManualQuizCheckQueueItemById(queueItemId);
				if (!User.HasAccessFor(quizCheckQueueItem.CourseId, CourseRole.Instructor))
					return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
				if (quizCheckQueueItem.IsChecked)
					return RedirectToAction("ManualQuizChecksQueue",
						new
						{
							courseId = quizCheckQueueItem.CourseId,
							message = "already_checked"
						});
				if (quizCheckQueueItem.IsLocked && !ignoreLock && !quizCheckQueueItem.IsLockedBy(User.Identity))
					return RedirectToAction("ManualQuizChecksQueue",
							new
							{
								courseId = quizCheckQueueItem.CourseId,
								message = "locked"
							});

				await userQuizzesRepo.LockManualQuizCheckQueueItem(quizCheckQueueItem, User.Identity.GetUserId());
				transaction.Commit();
			}
			return RedirectToRoute("Course.SlideById", new
			{
				CourseId = quizCheckQueueItem.CourseId,
				SlideId = quizCheckQueueItem.SlideId,
				CheckQueueItemId = quizCheckQueueItem.Id
			});
		}

		public async Task<ActionResult> CheckQuiz(int id)
		{
			return await InternalCheckQuiz(id);
		}
		
		public async Task<ActionResult> CheckNextQuizForSlide(string courseId, Guid slideId)
		{
			using (var transaction = db.Database.BeginTransaction())
			{
				var queueItems = userQuizzesRepo.GetManualQuizCheckQueue(courseId, slideId);
				var itemToCheck = queueItems.FirstOrDefault(i => i.LockedById == null);
				if (itemToCheck == null)
					return RedirectToAction("ManualQuizChecksQueue", new { courseId = courseId, message = "slide_checked" });
				
				if (itemToCheck.CourseId != courseId)
					return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

				await userQuizzesRepo.LockManualQuizCheckQueueItem(itemToCheck, User.Identity.GetUserId());

				transaction.Commit();

				return await InternalCheckQuiz(itemToCheck.Id, true);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ULearnAuthorize(MinAccessLevel = CourseRole.CourseAdmin)]
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

		[ULearnAuthorize(MinAccessLevel = CourseRole.CourseAdmin)]
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

			model.UsersGroups = groupsRepo.GetUsersGroupsNamesAsStrings(courseId, model.Users.Select(u => u.UserId), User);

			return model;
		}

		[ULearnAuthorize(MinAccessLevel = CourseRole.CourseAdmin)]
		public ActionResult Diagnostics(string courseId, Guid? versionId)
		{
			if (versionId == null)
			{
				return View(new DiagnosticsModel
				{
					CourseId = courseId,
				});
			}

			var versionIdGuid = (Guid)versionId;

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
		[ULearnAuthorize(MinAccessLevel = CourseRole.CourseAdmin)]
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
		[ULearnAuthorize(MinAccessLevel = CourseRole.CourseAdmin)]
		public async Task<ActionResult> DeleteVersion(string courseId, Guid versionId)
		{
			/* Remove information from database */
			await coursesRepo.DeleteCourseVersion(courseId, versionId);

			/* Delete zip-archive from file system */
			courseManager.GetCourseVersionFile(versionId).Delete();

			return RedirectToAction("Packages", new { courseId });
		}

		public ActionResult Groups(string courseId)
		{
			var groups = groupsRepo.GetAvailableForUserGroups(courseId, User);

			return View("Groups", new GroupsViewModel
			{
				CourseId = courseId,
				Groups = groups,
			});
		}

		public ActionResult CreateGroup(string courseId, string name, bool isPublic)
		{
			var group = groupsRepo.CreateGroup(courseId, name, User.Identity.GetUserId(), isPublic);
			return RedirectToAction("Groups", new {courseId});
		}

		private bool CanModifyGroup(Group group)
		{
			var courseId = group.CourseId;
			if (groupsRepo.CanUserSeeAllCourseGroups(User, courseId))
				return true;
			return group.OwnerId == User.Identity.GetUserId() || group.IsPublic;
		}

		[HttpPost]
		public async Task<ActionResult> AddUserToGroup(int groupId, string userId)
		{
			var group = groupsRepo.GetGroupById(groupId);
			if (!CanModifyGroup(group))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
			var added = await groupsRepo.AddUserToGroup(groupId, userId);

			return Json(new {added});
		}

		[HttpPost]
		public async Task<ActionResult> RemoveUserFromGroup(int groupId, string userId)
		{
			var group = groupsRepo.GetGroupById(groupId);
			if (!CanModifyGroup(group))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
			await groupsRepo.RemoveUserFromGroup(groupId, userId);

			return Json(new { removed="true" });
		}

		[HttpPost]
		public async Task<ActionResult> UpdateGroup(int groupId, string name, bool isPublic)
		{
			var group = groupsRepo.GetGroupById(groupId);
			if (!CanModifyGroup(group))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
			await groupsRepo.ModifyGroup(groupId, name, isPublic);

			return RedirectToAction("Groups", new { courseId = group.CourseId });
		}

		[HttpPost]
		public async Task<ActionResult> RemoveGroup(int groupId)
		{
			var group = groupsRepo.GetGroupById(groupId);
			if (!CanModifyGroup(group))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
			await groupsRepo.RemoveGroup(groupId);

			return RedirectToAction("Groups", new { courseId = group.CourseId });
		}

		[HttpPost]
		public async Task<ActionResult> EnableGroupInviteLink(int groupId, bool isEnabled)
		{
			var group = groupsRepo.GetGroupById(groupId);
			if (!CanModifyGroup(group))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
			await groupsRepo.EnableGroupInviteLink(groupId, isEnabled);

			return RedirectToAction("Groups", new { courseId = group.CourseId });
		}

		public ActionResult FindUsers(string term)
		{
			var query = new UserSearchQueryModel { NamePrefix = term };
			var users = usersRepo.FilterUsers(query, userManager)
				.Take(10)
				.Select(ur => new {id=ur.UserId, value=$"{ur.UserVisibleName} ({ur.UserName})" })
				.ToList();
			return Json(users, JsonRequestBehavior.AllowGet);
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

	public class ManualQuizCheckQueueViewModel
	{
		public string CourseId { get; set; }
		public List<ManualQuizCheckQueueItemViewModel> Checks { get; set; }
		public string Message { get; set; }
	}

	public class ManualQuizCheckQueueItemViewModel
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

	public class GroupsViewModel
	{
		public string CourseId { get; set; }

		public List<Group> Groups;
	}
}