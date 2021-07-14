using System;
using System.IO;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Slides;

namespace Database
{
	public interface IWebCourseManager
	{
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
		bool TryCreateCourse(string courseId, string courseTitle, Guid firstVersionId);
		void EnsureVersionIsExtracted(Guid versionId);
		bool HasPackageFor(string courseId);
		void WaitWhileCourseIsLocked(string courseId);
		void MoveCourse(Course course, DirectoryInfo sourceDirectory, DirectoryInfo destinationDirectory);
		bool TryReloadCourse(string courseId);
		void ReloadCourseNotSafe(string courseId, bool notifyAboutErrors = true);
		void ExtractTempCourseChanges(string tempCourseId);
		bool TryCreateTempCourse(string courseId, string courseTitle, Guid firstVersionId);
		void NotifyCourseChanged(string courseId);
		FileInfo GenerateOrFindStudentZip(string courseId, Slide slide);
	}
}