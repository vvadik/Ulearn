using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Database.Extensions;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Ulearn.Common;
using Ulearn.Common.Extensions;
using Ulearn.Core.Extensions;

namespace Database.Repos
{
	/* TODO (andgein): This repo is not fully migrated to .NET Core and EF Core */
	public class CoursesRepo : ICoursesRepo
	{
		private readonly UlearnDb db;

		public CoursesRepo(UlearnDb db)
		{
			this.db = db;
		}

		public Task<List<string>> GetPublishedCourseIdsAsync()
		{
			return db.CourseVersions
				.Select(v => v.CourseId)
				.Distinct()
				.ToListAsync();
		}

		public Task<CourseVersion> GetPublishedCourseVersionAsync(string courseId)
		{
			return db.CourseVersions
				.Where(v => v.CourseId == courseId && v.PublishTime != null)
				.OrderByDescending(v => v.PublishTime)
				.FirstOrDefaultAsync();
		}

		public Task<List<CourseVersion>> GetCourseVersionsAsync(string courseId)
		{
			return db.CourseVersions
				.Where(v => v.CourseId == courseId)
				.OrderByDescending(v => v.LoadingTime)
				.ToListAsync();
		}

		public async Task<CourseVersion> AddCourseVersionAsync(string courseId, Guid versionId, string authorId,
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
			await db.SaveChangesAsync().ConfigureAwait(false);

			return courseVersion;
		}

		public async Task MarkCourseVersionAsPublishedAsync(Guid versionId)
		{
			var courseVersion = await db.CourseVersions.FindAsync(versionId).ConfigureAwait(false);
			if (courseVersion == null)
				return;

			courseVersion.PublishTime = DateTime.Now;
			await db.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task DeleteCourseVersionAsync(string courseId, Guid versionId)
		{
			var courseVersion = await db.CourseVersions.FindAsync(versionId).ConfigureAwait(false);
			if (courseVersion == null)
				return;

			if (!courseId.EqualsIgnoreCase(courseVersion.CourseId))
				return;

			db.CourseVersions.Remove(courseVersion);
			await db.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task<List<CourseVersion>> GetPublishedCourseVersionsAsync()
		{
			var courseVersions = await db.CourseVersions.ToListAsync().ConfigureAwait(false);
			return courseVersions
				.GroupBy(v => v.CourseId.ToLower())
				.Select(g => g.MaxBy(v => v.PublishTime))
				.ToList();
		}

		/* Course accesses */

		private async Task<List<CourseAccess>> GetActualEnabledCourseAccessesAsync(string courseId = null, string userId = null)
		{
			var queryable = db.CourseAccesses
				.Include(a => a.User)
				.Where(a => a.IsEnabled);
			if (courseId != null)
				queryable = queryable.Where(x => x.CourseId == courseId);
			if (userId != null)
				queryable = queryable.Where(x => x.UserId == userId);
			return (await queryable.ToListAsync().ConfigureAwait(false))
				.GroupBy(x => x.CourseId + x.UserId + x.AccessType, StringComparer.OrdinalIgnoreCase)
				.Select(gr => gr.OrderByDescending(x => x.Id))
				.Select(gr => gr.FirstOrDefault())
				.ToList();
		}

		public async Task<CourseAccess> GrantAccessAsync(string courseId, string userId, CourseAccessType accessType, string grantedById, string comment)
		{
			courseId = courseId.ToLower();
			var currentAccess = new CourseAccess
			{
				CourseId = courseId,
				UserId = userId,
				AccessType = accessType,
				GrantTime = DateTime.Now,
				GrantedById = grantedById,
				IsEnabled = true,
				Comment = comment
			};
			db.CourseAccesses.Add(currentAccess);

			await db.SaveChangesAsync().ConfigureAwait(false);
			return db.CourseAccesses.Include(a => a.GrantedBy).Single(a => a.Id == currentAccess.Id);
		}

		public bool CanRevokeAccess(string courseId, string userId, IPrincipal revokedBy)
		{
			return revokedBy.HasAccessFor(courseId, CourseRoleType.CourseAdmin);
		}

		public async Task<List<CourseAccess>> RevokeAccessAsync(string courseId, string userId, CourseAccessType accessType, string grantedById, string comment)
		{
			courseId = courseId.ToLower();
			var revoke = new CourseAccess
			{
				UserId = userId,
				GrantTime = DateTime.Now,
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

		public async Task<List<CourseAccess>> GetCourseAccessesAsync(string courseId)
		{
			return await GetActualEnabledCourseAccessesAsync(courseId: courseId);
		}

		public async Task<List<CourseAccess>> GetCourseAccessesAsync(string courseId, string userId)
		{
			return await GetActualEnabledCourseAccessesAsync(courseId: courseId, userId: userId);
		}

		public async Task<DefaultDictionary<string, List<CourseAccess>>> GetCoursesAccessesAsync(IEnumerable<string> coursesIds)
		{
			var courseAccesses = (await GetActualEnabledCourseAccessesAsync().ConfigureAwait(false))
				.Where(a => coursesIds.Contains(a.CourseId, StringComparer.OrdinalIgnoreCase))
				.GroupBy(a => a.CourseId)
				.ToDictionary(g => g.Key, g => g.ToList());
			return courseAccesses.ToDefaultDictionary();
		}

		public async Task<bool> HasCourseAccessAsync(string userId, string courseId, CourseAccessType accessType)
		{
			return (await GetActualEnabledCourseAccessesAsync(courseId: courseId, userId: userId).ConfigureAwait(false))
				.Any(a => a.AccessType == accessType);
		}

		public async Task<List<CourseAccess>> GetUserAccessesAsync(string userId)
		{
			return await GetActualEnabledCourseAccessesAsync(userId: userId).ConfigureAwait(false);
		}

		public async Task<List<string>> GetCoursesUserHasAccessTo(string userId, CourseAccessType accessType)
		{
			return (await GetActualEnabledCourseAccessesAsync(userId: userId).ConfigureAwait(false))
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

		public Task<CourseFile> GetCourseFileAsync(string courseId)
		{
			return db.CourseFiles.FirstOrDefaultAsync(f => f.CourseId.Equals(courseId, StringComparison.OrdinalIgnoreCase));
		}

		public Task<List<CourseFile>> GetCourseFilesAsync(IEnumerable<string> exceptCourseIds)
		{
			return db.CourseFiles.Where(a => !exceptCourseIds.Contains(a.CourseId)).ToListAsync();
		}
	}
}