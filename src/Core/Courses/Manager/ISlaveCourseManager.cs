using System;
using System.IO;
using System.Threading.Tasks;
using Ulearn.Common;

namespace Ulearn.Core.Courses.Manager
{
	public interface ISlaveCourseManager : ICourseUpdater
	{
		Task<TempDirectory> ExtractCourseVersionToTemporaryDirectory(string courseId, Guid versionId, byte[] zipContent);
		(Course Course, Exception Exception) LoadCourseFromDirectory(string courseId, Guid versionId, DirectoryInfo extractedCourseDirectory);
		TempFile SaveVersionZipToTemporaryDirectory(string courseId, Guid versionId, Stream stream);
		Task<bool> CreateCourseIfNotExists(string courseId, Guid versionId, string courseTitle, string userId);
	}
}