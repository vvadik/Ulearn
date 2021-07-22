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
		FileInfo GetStagingCourseFile(string courseId);
		FileInfo GetStagingTempCourseFile(string courseId);
		DirectoryInfo GetExtractedCourseDirectory(string courseId);
		FileInfo GetCourseVersionFile(Guid versionId);
		void WaitWhileCourseIsLocked(string courseId);
		void ReloadCourseNotSafe(string courseId, bool notifyAboutErrors = true);
		void ExtractTempCourseChanges(string tempCourseId);
		bool TryCreateTempCourse(string courseId, string courseTitle, Guid firstVersionId);
		FileInfo GenerateOrFindStudentZip(string courseId, Slide slide);
	}
}