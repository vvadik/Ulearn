using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class CoursesRepo
	{
		private readonly ULearnDb db;

		public CoursesRepo() : this(new ULearnDb())
		{

		}

		public CoursesRepo(ULearnDb db)
		{
			this.db = db;
		}

		public CourseVersion GetPublishedCourseVersion(string courseId)
		{
			return db.CourseVersions.Where(v => v.CourseId == courseId && v.PublishTime != null).OrderByDescending(v => v.PublishTime).FirstOrDefault();
		}

		public IEnumerable<CourseVersion> GetCourseVersions(string courseId)
		{
			return db.CourseVersions.Where(v => v.CourseId == courseId).OrderByDescending(v => v.LoadingTime);
		}

		public async Task<CourseVersion> AddCourseVersion(string courseId, Guid versionId, string authorId)
		{
			var courseVersion = new CourseVersion
			{
				Id = versionId,
				CourseId = courseId,
				LoadingTime = DateTime.Now,
				PublishTime = null,
				AuthorId = authorId,
			};
			db.CourseVersions.Add(courseVersion);
			await db.SaveChangesAsync();

			return courseVersion;
		}

		public async Task MarkCourseVersionAsPublished(Guid versionId)
		{
			var courseVersion = db.CourseVersions.Find(versionId);
			if (courseVersion == null)
				return;

			courseVersion.PublishTime = DateTime.Now;
			await db.SaveChangesAsync();
		}

		public async Task DeleteCourseVersion(string courseId, Guid versionId)
		{
			var courseVersion = db.CourseVersions.Find(versionId);
			if (courseVersion == null)
				return;

			if (string.Compare(courseVersion.CourseId, courseId, StringComparison.OrdinalIgnoreCase) != 0)
				return;

			db.CourseVersions.Remove(courseVersion);
			await db.SaveChangesAsync();
		}
	}
}