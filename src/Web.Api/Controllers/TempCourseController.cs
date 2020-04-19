﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
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

		public TempCourseController(INotificationsRepo notificationsRepo, ICoursesRepo coursesRepo, ILogger logger, IWebCourseManager courseManager, UlearnDb db, [CanBeNull] IUsersRepo usersRepo, ITempCoursesRepo tempCoursesRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			this.notificationsRepo = notificationsRepo;
			this.coursesRepo = coursesRepo;
			this.tempCoursesRepo = tempCoursesRepo;
		}

		// todo не тестировал этот метод
		[HttpPost("create/{courseId}")]
		public async Task CreateCourse([FromRoute] string courseId)
		{
			//todo проверять права курс админа в courseId
			var userId = User.Identity.GetUserId();

			//var tmpCourseId = courseId + userId;

			var versionId = Guid.NewGuid();

			var courseTitle = "Temp course";

			if (!courseManager.TryCreateCourse(courseId, courseTitle, versionId))
				throw new Exception();
			
			await coursesRepo.AddCourseVersionAsync(courseId, versionId, userId, null, null, null, null).ConfigureAwait(false);
			await coursesRepo.MarkCourseVersionAsPublishedAsync(versionId).ConfigureAwait(false);
			await tempCoursesRepo.AddTempCourse(courseId, userId);
			var courseFile = courseManager.GetStagingCourseFile(courseId);
			await coursesRepo.AddCourseFile(courseId, versionId, courseFile.ReadAllContent()).ConfigureAwait(false);
			await NotifyAboutPublishedCourseVersion(courseId, versionId, userId).ConfigureAwait(false);
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
		public async Task UploadCourse([FromRoute] string courseId, List<IFormFile> files)
		{
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

			var (versionId, error) = await UploadCourse(courseId, file.OpenReadStream().ToArray(), User.Identity.GetUserId()).ConfigureAwait(false);
			if (error != null)
			{
				var errorMessage = error.Message.ToLowerFirstLetter();
				while (error.InnerException != null)
				{
					errorMessage += $"\n\n{error.InnerException.Message}";
					error = error.InnerException;
				}
			}

			await PublishVersion(courseId, versionId);
			await tempCoursesRepo.UpdateTempCourseLoadingTime(courseId);
		}

		private async Task PublishVersion(string courseId, Guid versionId)
		{
			var versionFile = courseManager.GetCourseVersionFile(versionId);
			var courseFile = courseManager.GetStagingCourseFile(courseId);
			var oldCourse = courseManager.GetCourse(courseId);

			//await coursesRepo.AddCourseFile(courseId, versionId, courseFile.ReadAllContent()).ConfigureAwait(false);

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
			courseManager.MoveCourse(
				version,
				extractedVersionDirectory,
				extractedCourseDirectory);
			await coursesRepo.MarkCourseVersionAsPublishedAsync(versionId);
			await NotifyAboutPublishedCourseVersion(courseId, versionId, User.Identity.GetUserId());

			courseManager.UpdateCourseVersion(courseId, versionId);
			courseManager.ReloadCourse(courseId);
			

			//var courseDiff = new CourseDiff(oldCourse, version);
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
			try
			{
				/* Load version and put it into LRU-cache */
				updatedCourse = courseManager.GetVersion(versionId);
			}
			catch (Exception e)
			{
				logger.Warning($"Upload course exception '{courseId}'", e);
				return (versionId, e);
			}

			logger.Information($"Successfully update course files '{courseId}'");
			// if (pathToCourseXmlInRepo == null && uploadedFromRepoUrl != null)
			// {
			// 	var extractedVersionDirectory = courseManager.GetExtractedVersionDirectory(versionId);
			// 	pathToCourseXmlInRepo = extractedVersionDirectory.FullName == updatedCourse.CourseXmlDirectory.FullName
			// 		? ""
			// 		: updatedCourse.CourseXmlDirectory.FullName.Substring(extractedVersionDirectory.FullName.Length + 1);
			// }
			//
			// await coursesRepo.AddCourseVersionAsync(courseId, versionId, userId,
			// 	pathToCourseXmlInRepo, uploadedFromRepoUrl, null, null);
			await NotifyAboutCourseVersion(courseId, versionId, userId);
			try
			{
				
				var courseVersions = await coursesRepo.GetCourseVersionsAsync(courseId);
				//probably always empty
				var previousUnpublishedVersions = courseVersions.Where(v => v.PublishTime == null && v.Id != versionId).ToList();
				foreach (var unpublishedVersion in previousUnpublishedVersions)
					await DeleteVersion(courseId, unpublishedVersion.Id).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				logger.Warning("Error during delete previous unpublished versions", ex);
			}

			return (versionId, null);
		}

		private async Task DeleteVersion(string courseId, Guid versionId)
		{
			/* Remove notifications from database */
			//await notificationsRepo.RemoveNotifications(versionId);

			/* Remove information from database */
			await coursesRepo.DeleteCourseVersionAsync(courseId, versionId);

			/* Delete zip-archive from file system */
			courseManager.GetCourseVersionFile(versionId).Delete();
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