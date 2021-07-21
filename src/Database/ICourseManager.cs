using System;
using System.IO;
using Ulearn.Core.Courses;

namespace Database
{
	public interface IWebCourseManager
	{
		DirectoryInfo ExtractCourseVersionToTemporaryDirectory(string courseId, Guid versionId, byte[] zipContent);
		(Course Course, Exception Exception)  LoadCourseFromDirectory(string courseId, Guid versionId, DirectoryInfo extractedCourseDirectory);



		void UpdateCourseVersion(string courseId, Guid versionId);
		Course GetVersion(Guid versionId);
		FileInfo GetStagingCourseFile(string courseId);
		DirectoryInfo GetExtractedCourseDirectory(string courseId);
		DirectoryInfo GetExtractedVersionDirectory(Guid versionId);
		FileInfo GetCourseVersionFile(Guid versionId);
		string GetStagingCoursePath(string courseId);
		string GetPackageName(string courseId);
		bool TryCreateCourse(string courseId, string courseTitle, Guid firstVersionId);
		void EnsureVersionIsExtracted(Guid versionId);
		bool HasPackageFor(string courseId);
		void MoveCourse(Course course, DirectoryInfo sourceDirectory, DirectoryInfo destinationDirectory);
		bool TryReloadCourse(string courseId);
		void ReloadCourseNotSafe(string courseId, bool notifyAboutErrors = true);
	}
}