using System;
using System.IO;
using System.Threading.Tasks;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Manager;
using Ulearn.Core.Courses.Slides;

namespace Ulearn.Web.Api.Utils
{
	public interface IMasterCourseManager : ICourseUpdater
	{
		Task<bool> CreateCourseIfNotExists(string courseId, Guid versionId, string courseTitle, string userId);
		
		
		
		void UpdateCourseVersion(string courseId, Guid versionId);
		Course GetVersion(Guid versionId);
		FileInfo GetStagingCourseFile(string courseId);
		FileInfo GetStagingTempCourseFile(string courseId);
		DirectoryInfo GetExtractedCourseDirectory(string courseId);
		DirectoryInfo GetExtractedVersionDirectory(Guid versionId);
		FileInfo GetCourseVersionFile(Guid versionId);
		string GetStagingCoursePath(string courseId);
		string GetPackageName(string courseId);
		string GetPackageName(Guid versionId);
		void EnsureVersionIsExtracted(Guid versionId);
		void WaitWhileCourseIsLocked(string courseId);
		void MoveCourse(Course course, DirectoryInfo sourceDirectory, DirectoryInfo destinationDirectory);
		bool TryReloadCourse(string courseId);
		void ReloadCourseNotSafe(string courseId, bool notifyAboutErrors = true);
		void ExtractTempCourseChanges(string tempCourseId);
		bool TryCreateTempCourse(string courseId, string courseTitle, Guid firstVersionId);
		FileInfo GenerateOrFindStudentZip(string courseId, Slide slide);
	}
}