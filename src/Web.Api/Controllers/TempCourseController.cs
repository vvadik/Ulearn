using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
using Ulearn.Web.Api.Models.Responses.TempCourses;
using Ionic.Zip;


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

			//--delete if everything will work fine without versions
			//await coursesRepo.AddCourseVersionAsync(tmpCourseId, versionId, userId, null, null, null, null).ConfigureAwait(false);
			//await coursesRepo.MarkCourseVersionAsPublishedAsync(versionId).ConfigureAwait(false);
			//await NotifyAboutPublishedCourseVersion(tmpCourseId, versionId, userId).ConfigureAwait(false); 

			await tempCoursesRepo.AddTempCourse(tmpCourseId, userId);
			return Ok($"course with id {tmpCourseId} successfully created");
		}

		[Authorize]
		[HttpPost("hasCourse/{courseId}")]
		public async Task<HasTempCourseResponse> HasCourse([FromRoute] string courseId)
		{
			var userId = User.Identity.GetUserId();
			/*if (!await courseRolesRepo.HasUserAccessToCourseAsync(userId, courseId, CourseRoleType.CourseAdmin))
				return BadRequest($"You dont have a Course Admin access to {courseId} course");*/

			var tmpCourseId = courseId + userId;
			var tmpCourse = tempCoursesRepo.Find(tmpCourseId);
			var response = new HasTempCourseResponse();
			if (tmpCourse == null)
			{
				response.HasTempCourse = false;
			}
			else
			{
				response.HasTempCourse = true;
				response.LastUploadTime = tmpCourse.LoadingTime;
				response.MainCourseId = courseId;
				response.TempCourseId = tmpCourseId;
			}

			return response;
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

			var uploadingError = await UploadCourse(tmpCourseId, file.OpenReadStream().ToArray(), User.Identity.GetUserId()).ConfigureAwait(false);
			var filesToDelete = new List<string>();
			var error = await TryPublishChanges(tmpCourseId, filesToDelete);
			if (error != null)
			{
				await tempCoursesRepo.UpdateOrAddTempCourseError(tmpCourseId, error);
				return BadRequest(error);
			}

			await tempCoursesRepo.MarkTempCourseAsNotErrored(tmpCourseId);
			await tempCoursesRepo.UpdateTempCourseLoadingTime(tmpCourseId);
			return Ok($"course with id {tmpCourseId} successfully updated");
		}

		private async Task<string> TryPublishChanges(string courseId, List<string> filesToDelete)
		{
			var revertStructure = GetRevertStructure(courseId, filesToDelete);
			courseManager.ExtractTempCourseChanges(courseId);
			DeleteFiles(revertStructure.DeletedFiles);
			var extractedCourseDirectory = courseManager.GetExtractedCourseDirectory(courseId);
			try
			{
				var updated = courseManager.ReloadCourseFromDirectory(extractedCourseDirectory);
				courseManager.UpdateCourseVersion(courseId, Guid.Empty); // todo do something with version
			}
			catch (Exception error)
			{
				var errorMessage = error.Message.ToLowerFirstLetter();
				while (error.InnerException != null)
				{
					errorMessage += $"\n\n{error.InnerException.Message}";
					error = error.InnerException;
				}

				RevertCourse(revertStructure, courseId);
				return errorMessage;
			}

			return null;
		}

		private void DeleteFiles(List<FileContent> filesToDelete)
		{
			foreach (var file in filesToDelete)
			{
				System.IO.File.Delete(file.Path);
			}
		}

		private RevertStructure GetRevertStructure(string courseId, List<string> filesToDelete)
		{
			var revertStructure = new RevertStructure();
			var staging = courseManager.GetStagingTempCourseFile(courseId);
			var courseDirectory = courseManager.GetExtractedCourseDirectory(courseId);
			var pathPrefix = courseDirectory.FullName;
			var zip = ZipFile.Read(staging.FullName, new ReadOptions { Encoding = Encoding.UTF8 });
			var filesToChangeRelativePaths = zip.Entries
				.Where(x => !x.IsDirectory)
				.Select(x => x.FileName)
				.Select(x => x.Replace('/', '\\'));
			var courseFileRelativePaths = Directory
				.EnumerateFiles(courseDirectory.FullName, "*.*", SearchOption.AllDirectories)
				.Select(file => file.Substring(file.IndexOf(pathPrefix) + pathPrefix.Length + 1))
				.ToHashSet();
			foreach (var name in filesToChangeRelativePaths)
			{
				var filePath = pathPrefix + "\\" + name;
				if (courseFileRelativePaths.Contains(name))
				{
					var bytes = System.IO.File.ReadAllBytes(filePath);
					revertStructure.FilesBeforeChanges.Add(new FileContent(filePath, bytes));
				}
				else
				{
					revertStructure.AddedFiles.Add(filePath);
				}
			}

			foreach (var fileToDeleteRelativePath in filesToDelete)
			{
				var filePath = pathPrefix + "\\" + fileToDeleteRelativePath;
				if (courseFileRelativePaths.Contains(fileToDeleteRelativePath))
				{
					var bytes = System.IO.File.ReadAllBytes(filePath);
					revertStructure.DeletedFiles.Add(new FileContent(filePath, bytes));
				}
			}

			return revertStructure;
		}

		private class RevertStructure
		{
			public List<FileContent> FilesBeforeChanges = new List<FileContent>();
			public List<string> AddedFiles = new List<string>();
			public List<FileContent> DeletedFiles = new List<FileContent>();
		}

		private struct FileContent
		{
			public FileContent(string path, byte[] content)
			{
				Path = path;
				Content = content;
			}

			public string Path;
			public byte[] Content;
		}

		private void RevertCourse(RevertStructure revertStructure, string courseId)
		{
			//todo lock course?
			foreach (var editedFile in revertStructure.FilesBeforeChanges)
			{
				System.IO.File.WriteAllBytes(editedFile.Path, editedFile.Content);
			}

			foreach (var addedFile in revertStructure.AddedFiles)
			{
				System.IO.File.Delete(addedFile);
			}

			foreach (var deletedFile in revertStructure.DeletedFiles)
			{
				System.IO.File.WriteAllBytes(deletedFile.Path, deletedFile.Content);
			}
			
		}


		private async Task<Exception> UploadCourse(string courseId, byte[] content, string userId)
		{
			logger.Information($"Start upload course '{courseId}'");
			var stagingFile = courseManager.GetStagingTempCourseFile(courseId);
			System.IO.File.WriteAllBytes(stagingFile.FullName, content);
			return null;
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