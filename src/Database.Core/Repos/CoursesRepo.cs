using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Ulearn.Common;
using Ulearn.Common.Extensions;
using Ulearn.Core.Extensions;

namespace Database.Repos
{
	public class CoursesRepo : ICoursesRepo
	{
		private readonly UlearnDb db;

		public CoursesRepo(UlearnDb db)
		{
			this.db = db;
		}

		public async Task<List<string>> GetPublishedCourseIds()
		{
			return await db.CourseVersions
				.Select(v => v.CourseId)
				.Distinct()
				.ToListAsync();
		}

		public async Task<CourseVersion> GetPublishedCourseVersion(string courseId)
		{
			return await db.CourseVersions
				.Where(v => v.CourseId == courseId && v.PublishTime != null)
				.OrderByDescending(v => v.PublishTime)
				.FirstOrDefaultAsync();
		}

		public async Task<List<CourseVersion>> GetCourseVersions(string courseId)
		{
			return await db.CourseVersions
				.Where(v => v.CourseId == courseId)
				.OrderByDescending(v => v.LoadingTime)
				.ToListAsync();
		}

		public async Task<CourseVersion> AddCourseVersion(string courseId, Guid versionId, string authorId,
			string pathToCourseXml, string repoUrl, string commitHash, string description)
		{
			var courseVersion = new CourseVersion
			{
				Id = versionId,
				CourseId = courseId,
				LoadingTime = DateTime.Now,
				PublishTime = null,
				AuthorId = authorId,
				PathToCourseXml = pathToCourseXml,
				CommitHash = commitHash,
				Description = description,
				RepoUrl = repoUrl
			};
			db.CourseVersions.Add(courseVersion);
			await db.SaveChangesAsync();

			return courseVersion;
		}

		public async Task MarkCourseVersionAsPublished(Guid versionId)
		{
			var courseVersion = await db.CourseVersions.FindAsync(versionId);
			if (courseVersion == null)
				return;

			courseVersion.PublishTime = DateTime.Now;
			await db.SaveChangesAsync();
		}

		public async Task DeleteCourseVersion(string courseId, Guid versionId)
		{
			var courseVersion = await db.CourseVersions.FindAsync(versionId);
			if (courseVersion == null)
				return;

			if (!courseId.EqualsIgnoreCase(courseVersion.CourseId))
				return;

			db.CourseVersions.Remove(courseVersion);
			await db.SaveChangesAsync();
		}

		public async Task<List<CourseVersion>> GetPublishedCourseVersions()
		{
			var courseVersions = await db.CourseVersions.ToListAsync();
			return courseVersions
				.GroupBy(v => v.CourseId.ToLower())
				.Select(g => g.MaxBy(v => v.PublishTime))
				.ToList();
		}

		/* Course accesses */

		private async Task<List<CourseAccess>> GetActualEnabledCourseAccesses(string courseId = null, string userId = null)
		{
			var queryable = db.CourseAccesses
				.Include(a => a.User)
				.Where(a => a.IsEnabled);
			if (courseId != null)
				queryable = queryable.Where(x => x.CourseId == courseId);
			if (userId != null)
				queryable = queryable.Where(x => x.UserId == userId);
			return (await queryable.ToListAsync())
				.GroupBy(x => x.CourseId + x.UserId + x.AccessType, StringComparer.OrdinalIgnoreCase)
				.Select(gr => gr.OrderByDescending(x => x.Id))
				.Select(gr => gr.FirstOrDefault())
				.ToList();
		}

		public async Task<CourseAccess> GrantAccess(string courseId, string userId, CourseAccessType accessType, string grantedById, string comment)
		{
			courseId = courseId.ToLower();
			var currentAccess = new CourseAccess
			{
				CourseId = courseId,
				UserId = userId,
				AccessType = accessType,
				GrantTime = DateTime.Now.ToUniversalTime(),
				GrantedById = grantedById,
				IsEnabled = true,
				Comment = comment
			};
			db.CourseAccesses.Add(currentAccess);

			await db.SaveChangesAsync().ConfigureAwait(false);
			return await db.CourseAccesses.Include(a => a.GrantedBy).FirstOrDefaultAsync(a => a.Id == currentAccess.Id);
		}

		public async Task<List<CourseAccess>> RevokeAccess(string courseId, string userId, CourseAccessType accessType, string grantedById, string comment)
		{
			courseId = courseId.ToLower();
			var revoke = new CourseAccess
			{
				UserId = userId,
				GrantTime = DateTime.Now.ToUniversalTime(),
				GrantedById = grantedById,
				Comment = comment,
				IsEnabled = false,
				CourseId = courseId,
				AccessType = accessType
			};
			db.CourseAccesses.Add(revoke);

			await db.SaveChangesAsync();
			return new List<CourseAccess> { revoke };
		}

		public async Task<List<CourseAccess>> GetCourseAccesses(string courseId)
		{
			return await GetActualEnabledCourseAccesses(courseId: courseId);
		}

		public async Task<List<CourseAccess>> GetCourseAccesses(string courseId, string userId)
		{
			return await GetActualEnabledCourseAccesses(courseId: courseId, userId: userId);
		}

		public async Task<DefaultDictionary<string, List<CourseAccess>>> GetCoursesAccesses(IEnumerable<string> coursesIds)
		{
			var courseAccesses = (await GetActualEnabledCourseAccesses())
				.Where(a => coursesIds.Contains(a.CourseId, StringComparer.OrdinalIgnoreCase))
				.GroupBy(a => a.CourseId)
				.ToDictionary(g => g.Key, g => g.ToList());
			return courseAccesses.ToDefaultDictionary();
		}

		public async Task<bool> HasCourseAccess(string userId, string courseId, CourseAccessType accessType)
		{
			return (await GetActualEnabledCourseAccesses(courseId: courseId, userId: userId))
				.Any(a => a.AccessType == accessType);
		}

		public async Task<List<CourseAccess>> GetUserAccesses(string userId)
		{
			return await GetActualEnabledCourseAccesses(userId: userId);
		}

		public async Task<List<string>> GetCoursesUserHasAccessTo(string userId, CourseAccessType accessType)
		{
			return (await GetActualEnabledCourseAccesses(userId: userId))
				.Where(a => a.AccessType == accessType)
				.Select(a => a.CourseId)
				.Distinct().ToList();
		}

		// Add new and remove old course file
		public async Task AddCourseFile(string courseId, Guid versionId, byte[] content)
		{
			var file = new CourseFile
			{
				CourseId = courseId,
				CourseVersionId = versionId,
				File = content
			};
			db.CourseFiles.RemoveRange(db.CourseFiles.Where(f => f.CourseId.Equals(courseId, StringComparison.OrdinalIgnoreCase)));
			db.CourseFiles.Add(file);
			await db.SaveChangesAsync();
		}

		public async Task<CourseFile> GetCourseFile(string courseId)
		{
			return await db.CourseFiles.FirstOrDefaultAsync(f => f.CourseId.Equals(courseId, StringComparison.OrdinalIgnoreCase));
		}

		public async Task<List<string>> GetCourseIdsFromCourseFiles()
		{
			return await db.CourseFiles.Select(cf => cf.CourseId).ToListAsync();
		}

		// Итерирование выполняется лениво и должно быть закончено до выполнения любых других методов
		// AsNoTracking делает запрос ленивым
		// Запрос всего списка сразу приведет к переполнению памяти в базе.
		public IQueryable<CourseFile> GetCourseFilesLazyNotSafe(IEnumerable<string> existingOnDiskCourseIds)
		{
			return db.CourseFiles.Where(a => !existingOnDiskCourseIds.Contains(a.CourseId)).AsNoTracking();
		}
	}
}