using log4net;
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
using Microsoft.VisualBasic.FileIO;
using uLearn.Quizes;
using uLearn.Web.DataContexts;
using uLearn.Web.Extensions;
using uLearn.Web.FilterAttributes;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	[ULearnAuthorize(MinAccessLevel = CourseRole.Instructor)]
	public class AdminController : Controller
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(AdminController));

		private readonly WebCourseManager courseManager;
		private readonly ULearnDb db;
		private readonly UsersRepo usersRepo;
		private readonly CommentsRepo commentsRepo;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly QuizzesRepo quizzesRepo;
		private readonly CoursesRepo coursesRepo;
		private readonly GroupsRepo groupsRepo;
		private readonly SlideCheckingsRepo slideCheckingsRepo;
		private readonly UserSolutionsRepo userSolutionsRepo;
		private readonly CertificatesRepo certificatesRepo;

		public AdminController()
		{
			db = new ULearnDb();
			courseManager = WebCourseManager.Instance;
			usersRepo = new UsersRepo(db);
			commentsRepo = new CommentsRepo(db);
			userManager = new ULearnUserManager();
			quizzesRepo = new QuizzesRepo(db);
			coursesRepo = new CoursesRepo(db);
			groupsRepo = new GroupsRepo(db);
			slideCheckingsRepo = new SlideCheckingsRepo(db);
			userSolutionsRepo = new UserSolutionsRepo(db);
			certificatesRepo = new CertificatesRepo(db);
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
		public ActionResult SpellingErrors(Guid versionId)
		{
			var course = courseManager.GetVersion(versionId);
			courseManager.EnsureVersionIsExtracted(versionId);
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
								LikesCount = commentsLikes.GetOrDefault(c.Id),
								IsLikedByUser = commentsLikedByUser.Contains(c.Id),
								Replies = new List<CommentViewModel>(),
								CanEditAndDeleteComment = true,
								CanModerateComment = true,
								IsCommentVisibleForUser = true,
								ShowContextInformation = true,
								ContextSlideTitle = slide.Title,
								ContextParentComment = c.IsTopLevel() ? null : commentsById.ContainsKey(c.ParentCommentId) ? commentsById[c.ParentCommentId].Text : null,
							}).ToList()
			});
		}

		private ManualCheckingQueueFilterOptions GetManualCheckingFilterOptionsByGroup(string courseId, string groupId)
		{
			return ControllerUtils.GetFilterOptionsByGroup<ManualCheckingQueueFilterOptions>(groupsRepo, User, courseId, groupId);
		}

		private ActionResult ManualCheckingQueue<T>(string actionName, string viewName, string courseId, string groupId, bool done, string userId = "", Guid? slideId = null, string message = "") where T : AbstractManualSlideChecking
		{
			var MaxShownQueueSize = 500;
			var course = courseManager.GetCourse(courseId);

			var filterOptions = GetManualCheckingFilterOptionsByGroup(courseId, groupId);
			if (filterOptions.UsersIds == null)
				groupId = "all";

			if (!string.IsNullOrEmpty(userId))
				filterOptions.UsersIds = new List<string> { userId };
			if (slideId.HasValue)
				filterOptions.SlidesIds = new List<Guid> { slideId.Value };

			filterOptions.OnlyChecked = done;
			filterOptions.Count = MaxShownQueueSize + 1;
			var checkings = slideCheckingsRepo.GetManualCheckingQueue<T>(filterOptions).ToList();

			if (!checkings.Any() && !string.IsNullOrEmpty(message))
				return RedirectToAction(actionName, new { courseId, groupId });

			var groups = groupsRepo.GetAvailableForUserGroups(courseId, User);
			var reviews = slideCheckingsRepo.GetExerciseCodeReviewForCheckings(checkings.Select(c => c.Id));
			var submissionsIds = checkings.Select(c => (c as ManualExerciseChecking)?.SubmissionId).Where(s => s.HasValue).Select(s => s.Value);
			var solutions = userSolutionsRepo.GetSolutionsForSubmissions(submissionsIds);
			return View(viewName, new ManualCheckingQueueViewModel
			{
				CourseId = courseId,
				Checkings = checkings.Take(MaxShownQueueSize).Select(c => new ManualCheckingQueueItemViewModel
				{
					CheckingQueueItem = c,
					ContextSlideTitle = course.GetSlideById(c.SlideId).Title,
					ContextMaxScore = course.GetSlideById(c.SlideId).MaxScore,
					ContextReviews = reviews.GetOrDefault(c.Id, new List<ExerciseCodeReview>()),
					ContextExerciseSolution = c is ManualExerciseChecking ?
						solutions.GetOrDefault((c as ManualExerciseChecking).SubmissionId, "") :
						"",
				}).ToList(),
				Groups = groups,
				GroupId = groupId,
				Message = message,
				AlreadyChecked = done,
				ExistsMore = checkings.Count > MaxShownQueueSize,
				ShowFilterForm = string.IsNullOrEmpty(userId) && ! slideId.HasValue,
			});
		}

		public ActionResult ManualQuizCheckingQueue(string courseId, string groupId, bool done = false, string userId = "", Guid? slideId = null, string message = "")
		{
			return ManualCheckingQueue<ManualQuizChecking>("ManualQuizCheckingQueue", "ManualQuizCheckingQueue", courseId, groupId, done, userId, slideId, message);
		}

		public ActionResult ManualExerciseCheckingQueue(string courseId, string groupId, bool done = false, string userId = "", Guid? slideId = null, string message = "")
		{
			return ManualCheckingQueue<ManualExerciseChecking>("ManualExerciseCheckingQueue", "ManualExerciseCheckingQueue", courseId, groupId, done, userId, slideId, message);
		}

		private async Task<ActionResult> InternalManualCheck<T>(string courseId, string actionName, int queueItemId, bool ignoreLock = false, string groupId = "", bool recheck = false) where T : AbstractManualSlideChecking
		{
			T checking;
			using (var transaction = db.Database.BeginTransaction())
			{
				checking = slideCheckingsRepo.FindManualCheckingById<T>(queueItemId);
				if (checking == null)
					return RedirectToAction(actionName,
						new
						{
							courseId = courseId,
							groupId = groupId,
							done = recheck,
							message = "already_checked",
						});

				if (!User.HasAccessFor(checking.CourseId, CourseRole.Instructor))
					return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

				if (checking.IsChecked && ! recheck)
					return RedirectToAction(actionName,
						new
						{
							courseId = checking.CourseId,
							groupId = groupId,
							done = recheck,
							message = "already_checked",
						});

				if (checking.IsLocked && !ignoreLock && !checking.IsLockedBy(User.Identity))
					return RedirectToAction(actionName,
						new
						{
							courseId = checking.CourseId,
							groupId = groupId,
							done = recheck,
							message = "locked",
						});

				await slideCheckingsRepo.LockManualChecking(checking, User.Identity.GetUserId());
				transaction.Commit();
			}

			return RedirectToRoute("Course.SlideById", new
			{
				checking.CourseId,
				checking.SlideId,
				CheckQueueItemId = checking.Id,
				GroupId = groupId,
			});
		}

		private async Task<ActionResult> CheckNextManualCheckingForSlide<T>(string actionName, string courseId, Guid slideId, string groupId) where T : AbstractManualSlideChecking
		{
			using (var transaction = db.Database.BeginTransaction())
			{
				var filterOptions = GetManualCheckingFilterOptionsByGroup(courseId, groupId);
				if (filterOptions.UsersIds == null)
					groupId = "all";
				filterOptions.SlidesIds = new List<Guid> { slideId };
				var checkings = slideCheckingsRepo.GetManualCheckingQueue<T>(filterOptions).ToList();

				var itemToCheck = checkings.FirstOrDefault(i => ! i.IsLocked);
				if (itemToCheck == null)
					return RedirectToAction(actionName, new { courseId, groupId, message = "slide_checked" });
				
				await slideCheckingsRepo.LockManualChecking(itemToCheck, User.Identity.GetUserId());

				transaction.Commit();

				return await InternalManualCheck<T>(courseId, actionName, itemToCheck.Id, true, groupId);
			}
		}

		public async Task<ActionResult> CheckQuiz(string courseId, int id, string groupId, bool recheck = false)
		{
			return await InternalManualCheck<ManualQuizChecking>(courseId, "ManualQuizCheckingQueue", id, false, groupId, recheck);
		}

		public async Task<ActionResult> CheckExercise(string courseId, int id, string groupId, bool recheck=false)
		{
			return await InternalManualCheck<ManualExerciseChecking>(courseId, "ManualExerciseCheckingQueue", id, false, groupId, recheck);
		}

		public async Task<ActionResult> CheckNextQuizForSlide(string courseId, Guid slideId, string groupId)
		{
			return await CheckNextManualCheckingForSlide<ManualQuizChecking>("ManualQuizCheckingQueue", courseId, slideId, groupId);
		}

		public async Task<ActionResult> CheckNextExerciseForSlide(string courseId, Guid slideId, string groupId)
		{
			return await CheckNextManualCheckingForSlide<ManualExerciseChecking>("ManualExerciseCheckingQueue", courseId, slideId, groupId);
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

		public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
		{
			/* Check that one directory is not a parent of another one */
			if (source.FullName.StartsWith(target.FullName) || target.FullName.StartsWith(source.FullName))
				throw new Exception("Can\'t copy files recursifely from parent to child directory or from child to parent");

			foreach (var subDirectory in source.GetDirectories())
				CopyFilesRecursively(subDirectory, target.CreateSubdirectory(subDirectory.Name));
			foreach (var file in source.GetFiles())
				file.CopyTo(Path.Combine(target.FullName, file.Name), true);
		}

		[HttpPost]
		[ULearnAuthorize(MinAccessLevel = CourseRole.CourseAdmin)]
		public async Task<ActionResult> PublishVersion(string courseId, Guid versionId)
		{
			log.Info($"Публикую версию курса {courseId}. ID версии: {versionId}");
			var versionFile = courseManager.GetCourseVersionFile(versionId);
			var courseFile = courseManager.GetStagingCourseFile(courseId);
			var oldCourse = courseManager.GetCourse(courseId);

			/* First, try to load course from LRU-cache or zip file */
			log.Info($"Загружаю версию {versionId}");
			var version = courseManager.GetVersion(versionId);

			/* Copy version's zip file to course's zip archive, overwrite if need */
			log.Info($"Копирую архив с версий в архив с курсом: {versionFile.FullName} → {courseFile.FullName}");
			versionFile.CopyTo(courseFile.FullName, true);
			courseManager.EnsureVersionIsExtracted(versionId);

			/* Replace courseId */
			version.Id = courseId;

			/* and move course from version's directory to courses's directory */
			var extractedVersionDirectory = courseManager.GetExtractedVersionDirectory(versionId);
			var extractedCourseDirectory = courseManager.GetExtractedCourseDirectory(courseId);
			log.Info($"Перемещаю паку с версий в папку с курсом: {extractedVersionDirectory.FullName} → {extractedCourseDirectory.FullName}");
			courseManager.MoveCourse(
				version,
				extractedVersionDirectory,
				extractedCourseDirectory);

			log.Info($"Создаю версии тестов для курса {courseId}");
			CreateQuizVersionsForSlides(courseId, version.Slides);
			log.Info($"Помечаю версию {versionId} как опубликованную версию курса {courseId}");
			await coursesRepo.MarkCourseVersionAsPublished(versionId);
			log.Info($"Обновляю курс {courseId} в оперативной памяти");
			courseManager.UpdateCourseVersion(courseId, versionId);

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
			var course = courseManager.GetCourse(courseId);

			return View("Groups", new GroupsViewModel
			{
				CourseId = courseId,
				CourseManualCheckingEnabled = course.Settings.IsManualCheckingEnabled,
				Groups = groups,
			});
		}

		public ActionResult CreateGroup(string courseId, string name, bool isPublic, bool manualChecking)
		{
			var group = groupsRepo.CreateGroup(courseId, name, User.Identity.GetUserId(), isPublic, manualChecking);
			return RedirectToAction("Groups", new {courseId});
		}

		private bool CanSeeAndModifyGroup(Group group)
		{
			var courseId = group.CourseId;
			if (groupsRepo.CanUserSeeAllCourseGroups(User, courseId))
				return true;
			return group.OwnerId == User.Identity.GetUserId() || group.IsPublic;
		}

		[HttpPost]
		public async Task<ActionResult> AddUserToGroup(int groupId, string userId)
		{
			var group = groupsRepo.FindGroupById(groupId);
			if (!CanSeeAndModifyGroup(group))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
			var added = await groupsRepo.AddUserToGroup(groupId, userId);

			return Json(new {added});
		}

		[HttpPost]
		public async Task<ActionResult> RemoveUserFromGroup(int groupId, string userId)
		{
			var group = groupsRepo.FindGroupById(groupId);
			if (!CanSeeAndModifyGroup(group))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
			await groupsRepo.RemoveUserFromGroup(groupId, userId);

			return Json(new { removed="true" });
		}

		[HttpPost]
		public async Task<ActionResult> UpdateGroup(int groupId, string name, bool isPublic, bool manualChecking)
		{
			var group = groupsRepo.FindGroupById(groupId);
			if (!CanSeeAndModifyGroup(group))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
			await groupsRepo.ModifyGroup(groupId, name, isPublic, manualChecking);

			return RedirectToAction("Groups", new { courseId = group.CourseId });
		}

		[HttpPost]
		public async Task<ActionResult> RemoveGroup(int groupId)
		{
			var group = groupsRepo.FindGroupById(groupId);
			if (!CanSeeAndModifyGroup(group))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
			await groupsRepo.RemoveGroup(groupId);

			return RedirectToAction("Groups", new { courseId = group.CourseId });
		}

		[HttpPost]
		public async Task<ActionResult> EnableGroupInviteLink(int groupId, bool isEnabled)
		{
			var group = groupsRepo.FindGroupById(groupId);
			if (!CanSeeAndModifyGroup(group))
				return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
			await groupsRepo.EnableGroupInviteLink(groupId, isEnabled);

			return RedirectToAction("Groups", new { courseId = group.CourseId });
		}

		public ActionResult FindUsers(string term)
		{
			var query = new UserSearchQueryModel { NamePrefix = term };
			var users = usersRepo.FilterUsers(query, userManager)
				.Take(10)
				.Select(ur => new { id=ur.UserId, value=$"{ur.UserVisibleName} ({ur.UserName})" })
				.ToList();
			return Json(users, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Certificates(string courseId)
		{
			var course = courseManager.GetCourse(courseId);
			var certificateTemplates = certificatesRepo.GetTemplates(courseId).ToDictionary(t => t.Id);
			var certificates = certificatesRepo.GetCertificates(courseId);
			var templateParameters = certificateTemplates.ToDictionary(
				kv => kv.Key,
				kv => certificatesRepo.GetTemplateParametersWithoutBuiltins(kv.Value).ToList()
			);

			return View(new CertificatesViewModel
			{
				Course = course,
				Templates = certificateTemplates,
				TemplateParameters = templateParameters,
				Certificates = certificates
			});
		}

		[HttpPost]
		[ULearnAuthorize(MinAccessLevel=CourseRole.CourseAdmin)]
		public async Task<ActionResult> CreateCertificateTemplate(string courseId, string name, HttpPostedFileBase archive)
		{
			if (archive == null || archive.ContentLength <= 0)
			{
				log.Error("Создание шаблона сертификата: ошибка загрузки архива");
				throw new Exception("Ошибка загрузки архива");
			}
			
			var archiveName = SaveUploadedTemplate(archive);
			await certificatesRepo.AddTemplate(courseId, name, archiveName);

			return RedirectToAction("Certificates", new { courseId });
		}

		private string SaveUploadedTemplate(HttpPostedFileBase archive)
		{
			var archiveName = Utils.NewNormalizedGuid();
			var templateArchivePath = certificatesRepo.GetTemplateArchivePath(archiveName);
			try
			{
				archive.SaveAs(templateArchivePath.FullName);
			}
			catch (Exception e)
			{
				log.Error("Создание шаблона сертификата: не могу сохранить архив", e);
				throw;
			}
			return archiveName;
		}

		[HttpPost]
		[ULearnAuthorize(MinAccessLevel=CourseRole.CourseAdmin)]
		public async Task<ActionResult> EditCertificateTemplate(string courseId, Guid templateId, string name, HttpPostedFileBase archive)
		{
			var template = certificatesRepo.FindTemplateById(templateId);
			if (template == null || template.CourseId != courseId)
				return HttpNotFound();

			if (archive != null && archive.ContentLength > 0)
			{
				var archiveName = SaveUploadedTemplate(archive);
				await certificatesRepo.ChangeTemplateArchiveName(templateId, archiveName);
			}

			await certificatesRepo.ChangeTemplateName(templateId, name);

			return RedirectToAction("Certificates", new { courseId });
		}

		[HttpPost]
		[ULearnAuthorize(MinAccessLevel=CourseRole.CourseAdmin)]
		public async Task<ActionResult> RemoveCertificateTemplate(string courseId, Guid templateId)
		{
			var template = certificatesRepo.FindTemplateById(templateId);
			if (template == null || template.CourseId != courseId)
				return HttpNotFound();

			await certificatesRepo.RemoveTemplate(template);

			return RedirectToAction("Certificates", new { courseId });
		}
		
		[HttpPost]
		[ValidateInput(false)]
		public async Task<ActionResult> AddCertificate(string courseId, Guid templateId, string userId, bool isPreview=false)
		{
			var template = certificatesRepo.FindTemplateById(templateId);
			if (template == null || template.CourseId != courseId)
				return HttpNotFound();

			var certificateParameters = GetCertificateParametersFromRequest(template);
			if (certificateParameters == null)
				return RedirectToAction("Certificates", new { courseId });

			var certificate = await certificatesRepo.AddCertificate(templateId, userId, User.Identity.GetUserId(), certificateParameters, isPreview);

			if (isPreview)
				return RedirectToRoute("Certificate", new { certificateId = certificate.Id });
			return Redirect(Url.Action("Certificates", new { courseId }) + "#template-" + templateId);
		}

		private Dictionary<string, string> GetCertificateParametersFromRequest(CertificateTemplate template)
		{
			var templateParameters = certificatesRepo.GetTemplateParametersWithoutBuiltins(template);
			var certificateParameters = new Dictionary<string, string>();
			foreach (var parameter in templateParameters)
			{
				if (Request.Unvalidated.Form["parameter-" + parameter] == null)
					return null;
				certificateParameters[parameter] = Request.Unvalidated.Form["parameter-" + parameter];
			}
			return certificateParameters;
		}

		[HttpPost]
		public async Task<ActionResult> RemoveCertificate(string courseId, Guid certificateId)
		{
			var certificate = certificatesRepo.FindCertificateById(certificateId);
			if (certificate == null)
				return HttpNotFound();

			if (!User.HasAccessFor(certificate.Template.CourseId, CourseRole.CourseAdmin) &&
				certificate.InstructorId != User.Identity.GetUserId())
				return HttpNotFound();

			await certificatesRepo.RemoveCertificate(certificate);
			return Json(new { status = "ok" });
		}

		public ActionResult DownloadCertificateTemplate(string courseId, Guid templateId)
		{
			var template = certificatesRepo.FindTemplateById(templateId);
			if (template == null || template.CourseId != courseId)
				return HttpNotFound();

			return RedirectPermanent($"/Certificates/{template.ArchiveName}.zip");
		}

		public ActionResult PreviewCertificates(string courseId, Guid templateId, HttpPostedFileBase certificatesData)
		{
			const string namesColumnName = "Фамилия Имя";

			var template = certificatesRepo.FindTemplateById(templateId);
			if (template == null || template.CourseId != courseId)
				return HttpNotFound();

			var notBuiltinTemplateParameters = certificatesRepo.GetTemplateParametersWithoutBuiltins(template).ToList();
			var builtinTemplateParameters = certificatesRepo.GetBuiltinTemplateParameters(template).ToList();
			builtinTemplateParameters.Sort();

			var model = new PreviewCertificatesViewModel
			{
				CourseId = courseId,
				Template = template,
				NotBuiltinTemplateParameters = notBuiltinTemplateParameters,
				BuiltinTemplateParameters = builtinTemplateParameters,
				Certificates = new List<PreviewCertificatesCertificateModel>(),
			};

			if (certificatesData == null || certificatesData.ContentLength <= 0)
			{
				return View(model.WithError("Ошибка загрузки файла с данными для сертификатов"));
			}
			
			using (var parser = new TextFieldParser(new StreamReader(certificatesData.InputStream)))
			{
				parser.TextFieldType = FieldType.Delimited;
				parser.SetDelimiters(",");
				if (parser.EndOfData)
					return View(model.WithError("Пустой файл? В файле с данными должна присутствовать строка к заголовком"));

				string[] headers;
				try
				{
					headers = parser.ReadFields();
				}
				catch (MalformedLineException e)
				{
					return View(model.WithError($"Ошибка в файле с данными: в строке {parser.ErrorLineNumber}: \"{parser.ErrorLine}\". {e}"));
				}
				var namesColumnIndex = headers.FindIndex(namesColumnName);
				if (namesColumnIndex < 0)
					return View(model.WithError($"Одно из полей должно иметь имя \"{namesColumnName}\", в нём должны содержаться фамилия и имя студента. Смотрите пример файла с данными"));

				var parametersIndeces = new Dictionary<string, int>();
				foreach (var parameter in notBuiltinTemplateParameters)
				{
					parametersIndeces[parameter] = headers.FindIndex(parameter);
					if (parametersIndeces[parameter] < 0)
						return View(model.WithError($"Одно из полей должно иметь имя \"{parameter}\", в нём должна содержаться подстановка для шаблона. Смотрите пример файла с данными"));
				}

				while (!parser.EndOfData)
				{
					string[] fields;
					try
					{
						fields = parser.ReadFields();
					}
					catch (MalformedLineException e)
					{
						return View(model.WithError($"Ошибка в файле с данными: в строке {parser.ErrorLineNumber}: \"{parser.ErrorLine}\". {e}"));
					}
					if (fields.Length != headers.Length)
					{
						return View(model.WithError($"Ошибка в файле с данными: в строке {parser.ErrorLineNumber}: \"{parser.ErrorLine}\". Количество ячеек в строке не совпадает с количеством столбцов в заголовке"));
					}

					var userNames = fields[namesColumnIndex];

					var query = new UserSearchQueryModel { NamePrefix = userNames };
					var users = usersRepo.FilterUsers(query, userManager)
						.Take(10)
						.ToList();

					var exportedParameters = parametersIndeces.ToDictionary(kv => kv.Key, kv => fields[kv.Value]);
					var certificateModel = new PreviewCertificatesCertificateModel
					{
						UserNames = userNames,
						Users = users,
						Parameters = exportedParameters,
					};
					model.Certificates.Add(certificateModel);
				}
			}

			return View(model);
		}

		public async Task<ActionResult> GenerateCertificates(string courseId, Guid templateId, int maxCertificateId)
		{
			var template = certificatesRepo.FindTemplateById(templateId);
			if (template == null || template.CourseId != courseId)
				return HttpNotFound();

			var templateParameters = certificatesRepo.GetTemplateParametersWithoutBuiltins(template).ToList();
			var certificateRequests = new List<CertificateRequest>();

			for (var certificateIndex = 0; certificateIndex < maxCertificateId; certificateIndex++)
			{
				var userId = Request.Form.Get($"user-{certificateIndex}");
				if (userId == null)
					continue;
				if (string.IsNullOrEmpty(userId))
				{
					return View("GenerateCertificatesError", (object) "Не все пользователи выбраны");
				}

				var parameters = new Dictionary<string, string>();
				foreach (var parameterName in templateParameters)
				{
					var parameterValue = Request.Unvalidated.Form.Get($"parameter-{certificateIndex}-{parameterName}");
					parameters[parameterName] = parameterValue;
				}

				certificateRequests.Add(new CertificateRequest
				{
					UserId = userId,
					Parameters = parameters,
				});
			}

			foreach (var certificateRequest in certificateRequests)
			{
				await certificatesRepo.AddCertificate(templateId, certificateRequest.UserId, User.Identity.GetUserId(), certificateRequest.Parameters);
			}

			return Redirect(Url.Action("Certificates", new { courseId }) + "#template-" + templateId);
		}

		public async Task<ActionResult> GetBuiltinCertificateParametersForUser(string courseId, Guid templateId, string userId)
		{
			var template = certificatesRepo.FindTemplateById(templateId);
			if (template == null || template.CourseId != courseId)
				return HttpNotFound();

			var user = await userManager.FindByIdAsync(userId);
			var instructor = await userManager.FindByIdAsync(User.Identity.GetUserId());
			var course = courseManager.GetCourse(courseId);

			var builtinParameters = certificatesRepo.GetBuiltinTemplateParameters(template);
			var builtinParametersValues = builtinParameters.ToDictionary(
				p => p,
				p => certificatesRepo.GetTemplateBuiltinParameterForUser(template, course, user, instructor, p)
			);

			return Json(builtinParametersValues, JsonRequestBehavior.AllowGet);
		}
	}

	public class CertificateRequest
	{
		public string UserId;
		public Dictionary<string, string> Parameters;
	}

	public class PreviewCertificatesCertificateModel
	{
		public string UserNames { get; set; }
		public List<UserRolesInfo> Users { get; set; }
		public Dictionary<string, string> Parameters { get; set; }
	}

	public class PreviewCertificatesViewModel
	{
		public string Error { get; set; }
		
		public string CourseId { get; set; }
		public CertificateTemplate Template { get; set; }
		public List<PreviewCertificatesCertificateModel> Certificates { get; set; }
		public List<string> NotBuiltinTemplateParameters { get; set; }
		public List<string> BuiltinTemplateParameters { get; set; }

		public PreviewCertificatesViewModel WithError(string error)
		{
			return new PreviewCertificatesViewModel
			{
				CourseId = CourseId,
				Template = Template,
				Error = error,
			};
		}
	}

	public class CertificatesViewModel
	{
		public Course Course { get; set; }

		public Dictionary<Guid, CertificateTemplate> Templates { get; set; }
		public Dictionary<Guid, List<Certificate>> Certificates { get; set; }
		public Dictionary<Guid, List<string>> TemplateParameters { get; set; }
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

	public class ManualCheckingQueueViewModel
	{
		public string CourseId { get; set; }
		public List<ManualCheckingQueueItemViewModel> Checkings { get; set; }
		public string Message { get; set; }
		public List<Group> Groups { get; set; }
		public string GroupId { get; set; }
		public bool AlreadyChecked { get; set; }
		public bool ExistsMore { get; set; }
		public bool ShowFilterForm { get; set; }
	}

	public class ManualCheckingQueueItemViewModel
	{
		public AbstractManualSlideChecking CheckingQueueItem { get; set; }

		public string ContextSlideTitle { get; set; }
		public int ContextMaxScore { get; set; }
		public List<ExerciseCodeReview> ContextReviews { get; set; }
		public string ContextExerciseSolution { get; set; }
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
		public bool CourseManualCheckingEnabled { get; set; }

		public List<Group> Groups;
	}
}