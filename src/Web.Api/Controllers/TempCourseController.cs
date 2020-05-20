using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.CourseRoles;
using Database.Repos.Users;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses;

namespace Ulearn.Web.Api.Controllers
{
	[Route("/tempCourses")]
	public class TempCourseController : BaseController
	{
		private readonly ICoursesRepo coursesRepo;
		private readonly ITempCoursesRepo tempCoursesRepo;
		private readonly INotificationsRepo notificationsRepo;
		private readonly ICourseRolesRepo courseRolesRepo;

		public TempCourseController(INotificationsRepo notificationsRepo, ICoursesRepo coursesRepo, ILogger logger, IWebCourseManager courseManager, UlearnDb db, [CanBeNull] IUsersRepo usersRepo, ITempCoursesRepo tempCoursesRepo, ICourseRolesRepo courseRolesRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			this.notificationsRepo = notificationsRepo;
			this.coursesRepo = coursesRepo;
			this.tempCoursesRepo = tempCoursesRepo;
			this.courseRolesRepo = courseRolesRepo;
		}

		[Authorize]
		[HttpPost("create/{courseId}")]
		public async Task<IActionResult> CreateCourse([FromRoute] string courseId)
		{
			var userId = User.Identity.GetUserId();
			/*if (!await courseRolesRepo.HasUserAccessToCourseAsync(userId, courseId, CourseRoleType.CourseAdmin))
				return BadRequest($"You dont have a Course Admin access to {courseId} course");*/

			var tmpCourseId = courseId + userId;
			var tmpCourse = tempCoursesRepo.Find(tmpCourseId);
			if (tmpCourse != null)
				return BadRequest($"Your temp version of course {courseId} already exists with id {tmpCourseId}");

			var versionId = Guid.NewGuid();

			var courseTitle = "Temp course";

			if (!courseManager.TryCreateCourse(tmpCourseId, courseTitle, versionId))
				throw new Exception();

			await coursesRepo.AddCourseVersionAsync(tmpCourseId, versionId, userId, null, null, null, null).ConfigureAwait(false);
			await coursesRepo.MarkCourseVersionAsPublishedAsync(versionId).ConfigureAwait(false);
			await tempCoursesRepo.AddTempCourse(tmpCourseId, userId);
			await NotifyAboutPublishedCourseVersion(tmpCourseId, versionId, userId).ConfigureAwait(false);
			return Ok($"course with id {tmpCourseId} successfully created");
		}

		private async Task NotifyAboutPublishedCourseVersion(string courseId, Guid versionId, string userId)
		{
			var notification = new PublishedPackageNotification
			{
				CourseVersionId = versionId,
			};
			await notificationsRepo.AddNotificationAsync(courseId, notification, userId);
		}


		[HttpPost("uploadCourse/{courseId}")]
		[Authorize]
		public async Task<IActionResult> UploadCourse([FromRoute] string courseId, List<IFormFile> files)
		{
			var userId = User.Identity.GetUserId();
			/*if (!await courseRolesRepo.HasUserAccessToCourseAsync(userId, courseId, CourseRoleType.CourseAdmin))
				return BadRequest($"You dont have a Course Admin access to {courseId} course");*/

			var tmpCourseId = courseId + userId;
			var tmpCourse = tempCoursesRepo.Find(tmpCourseId);
			if (tmpCourse is null)
				return BadRequest($"Your temp version of course {courseId} does not exists. Use create method");
			if (files.Count != 1)
			{
				throw new Exception();
			}

			var file = files.Single();
			if (file == null || file.Length <= 0)
				throw new Exception();

			var fileName = Path.GetFileName(file.FileName);
			if (fileName == null || !fileName.ToLower().EndsWith(".zip"))
				throw new Exception();

			var (versionId, uploadingError) = await UploadCourse(tmpCourseId, file.OpenReadStream().ToArray(), User.Identity.GetUserId()).ConfigureAwait(false);

			var error = await TryPublishChanges(tmpCourseId, versionId);
			if (error != null)
			{
				await tempCoursesRepo.UpdateOrAddTempCourseError(tmpCourseId, error);
				return BadRequest(error);
			}

			await tempCoursesRepo.MarkTempCourseAsNotErrored(tmpCourseId);
			await tempCoursesRepo.UpdateTempCourseLoadingTime(tmpCourseId);
			return Ok($"course with id {tmpCourseId} successfully updated");
		}

		private async Task<string> TryPublishChanges(string courseId, Guid versionId)
		{
			var versionFile = courseManager.GetCourseVersionFile(versionId);
			var courseFile = courseManager.GetStagingCourseFile(courseId);
			//var oldCourse = courseManager.GetCourse(courseId); //я закометил эту строчку не просто так, с ней не работает

			/* First, try to load course from LRU-cache or zip file */
			var version = courseManager.GetVersion(versionId);

			/* Copy version's zip file to course's zip archive, overwrite if need */
			versionFile.CopyTo(courseFile.FullName, true);
			courseManager.EnsureVersionIsExtracted(versionId);

			/* Replace courseId */
			version.Id = courseId;

			/* and move course from version's directory to courses's directory */
			var extractedVersionDirectory = courseManager.GetExtractedVersionDirectory(versionId);
			var extractedCourseDirectory = courseManager.GetExtractedCourseDirectory(courseId);

			//remember what in extractedVersionDirectory and what we should delete
			courseManager.CopyTempCourse(version, extractedVersionDirectory, extractedCourseDirectory);
			//and delete that should be deleted

			await coursesRepo.MarkCourseVersionAsPublishedAsync(versionId);
			await NotifyAboutPublishedCourseVersion(courseId, versionId, User.Identity.GetUserId());
			Course updated;
			try
			{
				updated = courseManager.ReloadCourseFromDirectory(extractedCourseDirectory);
				courseManager.UpdateCourseVersion(courseId, versionId);
			}
			catch (Exception error)
			{
				var errorMessage = error.Message.ToLowerFirstLetter();
				while (error.InnerException != null)
				{
					errorMessage += $"\n\n{error.InnerException.Message}";
					error = error.InnerException;
				}

				RevertCourse();
				return errorMessage;
			}


			//var courseDiff = new CourseDiff(oldCourse, version);
			return null;
		}

		private void RevertCourse()
		{
			throw new NotImplementedException();
		}


		private async Task<(Guid versionId, Exception error)> UploadCourse(string courseId, byte[] content, string userId)
		{
			logger.Information($"Start upload course '{courseId}'");
			//var versionId = Guid.NewGuid();
			//get versionId that already exist, not create new
			var versionId = (await coursesRepo.GetCourseVersionsAsync(courseId)).Single().Id;
			var destinationFile = courseManager.GetCourseVersionFile(versionId);
			System.IO.File.WriteAllBytes(destinationFile.FullName, content);
			Course updatedCourse;
			// try
			// {
			// 	/* Load version and put it into LRU-cache */
			// 	updatedCourse = courseManager.GetVersion(versionId);
			// }
			// catch (Exception e)
			// {
			// 	logger.Warning($"Upload course exception '{courseId}'", e);
			// 	return (versionId, e);
			// }

			logger.Information($"Successfully update course files '{courseId}'");

			await NotifyAboutCourseVersion(courseId, versionId, userId);

			return (versionId, null);
		}

		private async Task NotifyAboutCourseVersion(string courseId, Guid versionId, string userId)
		{
			var notification = new UploadedPackageNotification
			{
				CourseVersionId = versionId
			};
			await notificationsRepo.AddNotificationAsync(courseId, notification, userId);
		}
	}
}