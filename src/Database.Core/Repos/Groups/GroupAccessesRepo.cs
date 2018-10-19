using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Database.Extensions;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Ulearn.Common;
using Ulearn.Common.Extensions;

namespace Database.Repos.Groups
{
	/* This class is fully migrated to .NET Core and EF Core */
	public class GroupAccessesRepo : IGroupAccessesRepo
	{
		private readonly UlearnDb db;
		private readonly IGroupsRepo groupsRepo;
		private readonly ISystemAccessesRepo systemAccessesRepo;
		private readonly ICoursesRepo coursesRepo;
		private readonly IGroupMembersRepo groupMembersRepo;
		private readonly WebCourseManager courseManager;
		private readonly ILogger logger;

		public GroupAccessesRepo(
			UlearnDb db,
			IGroupsRepo groupsRepo, ISystemAccessesRepo systemAccessesRepo, ICoursesRepo coursesRepo, IGroupMembersRepo groupMembersRepo,
			WebCourseManager courseManager,
			ILogger logger
		)
		{
			this.db = db;
			this.groupsRepo = groupsRepo;
			this.systemAccessesRepo = systemAccessesRepo;
			this.coursesRepo = coursesRepo;
			this.groupMembersRepo = groupMembersRepo;
			this.courseManager = courseManager;
			this.logger = logger;
		}

		public async Task<GroupAccess> GrantAccessAsync(int groupId, string userId, GroupAccessType accessType, string grantedById)
		{
			var currentAccess = await db.GroupAccesses.FirstOrDefaultAsync(a => a.GroupId == groupId && a.UserId == userId).ConfigureAwait(false);
			if (currentAccess == null)
			{
				currentAccess = new GroupAccess
				{
					GroupId = groupId,
					UserId = userId,
				};
				db.GroupAccesses.Add(currentAccess);
			}
			currentAccess.AccessType = accessType;
			currentAccess.GrantedById = grantedById;
			currentAccess.GrantTime = DateTime.Now;
			currentAccess.IsEnabled = true;

			await db.SaveChangesAsync().ConfigureAwait(false);
			return db.GroupAccesses.Include(a => a.GrantedBy).Single(a => a.Id == currentAccess.Id);
		}

		public async Task<bool> CanRevokeAccessAsync(int groupId, string userId, ClaimsPrincipal revokedBy)
		{
			var revokedById = revokedBy.GetUserId();

			var group = await groupsRepo.FindGroupByIdAsync(groupId).ConfigureAwait(false) ?? throw new ArgumentNullException($"Can't find group with id={groupId}");
			if (group.OwnerId == revokedById || revokedBy.HasAccessFor(group.CourseId, CourseRole.CourseAdmin))
				return true;
			return db.GroupAccesses.Any(a => a.GroupId == groupId && a.UserId == userId && a.GrantedById == revokedById && a.IsEnabled);
		}

		public async Task<List<GroupAccess>> RevokeAccessAsync(int groupId, string userId)
		{
			var accesses = await db.GroupAccesses.Where(a => a.GroupId == groupId && a.UserId == userId).ToListAsync().ConfigureAwait(false);
			foreach (var access in accesses)
				access.IsEnabled = false;

			await db.SaveChangesAsync().ConfigureAwait(false);
			return accesses;
		}

		public Task<List<GroupAccess>> GetGroupAccessesAsync(int groupId)
		{
			return db.GroupAccesses.Include(a => a.User).Include(a => a.GrantedBy).Where(a => a.GroupId == groupId && a.IsEnabled && !a.User.IsDeleted).ToListAsync();
		}

		public async Task<DefaultDictionary<int, List<GroupAccess>>> GetGroupAccessesAsync(IEnumerable<int> groupsIds)
		{
			var groupIdsList = groupsIds.ToList();
			var groupAccesses = await db.GroupAccesses
				.Include(a => a.User)
				.Include(a => a.GrantedBy)
				.Where(a => groupIdsList.Contains(a.GroupId) && a.IsEnabled && !a.User.IsDeleted)
				.ToListAsync()
				.ConfigureAwait(false);

			return groupAccesses
				.GroupBy(a => a.GroupId)
				.ToDictionary(g => g.Key, g => g.ToList())
				.ToDefaultDictionary();
		}
		
		public async Task<bool> HasUserAccessToGroupAsync(int groupId, string userId)
		{
			var group = await groupsRepo.FindGroupByIdAsync(groupId).ConfigureAwait(false) ?? throw new ArgumentNullException($"Can't find group with id={groupId}");
			
			if (group.OwnerId == userId)
				return true;

			return await db.GroupAccesses.Where(a => a.GroupId == groupId && a.UserId == userId && a.IsEnabled).AnyAsync().ConfigureAwait(false);
		}
		
		public async Task<bool> IsGroupVisibleForUserAsync(int groupId, ClaimsPrincipal user)
		{
			var group = await groupsRepo.FindGroupByIdAsync(groupId).ConfigureAwait(false) ?? throw new ArgumentNullException($"Can't find group with id={groupId}");
			return await IsGroupVisibleForUserAsync(group, user).ConfigureAwait(false);
		}

		public async Task<bool> IsGroupVisibleForUserAsync(Group group, ClaimsPrincipal user)
		{
			/* Course admins and other privileged users can see all groups in the course */
			if (await CanUserSeeAllCourseGroupsAsync(user, group.CourseId).ConfigureAwait(false))
				return true;

			if (!user.HasAccessFor(group.CourseId, CourseRole.Instructor))
				return false;

			return await HasUserAccessToGroupAsync(group.Id, user.GetUserId()).ConfigureAwait(false);
		}

		public Task<List<Group>> GetAvailableForUserGroupsAsync(string courseId, ClaimsPrincipal user, bool onlyArchived=false)
		{
			if (!user.HasAccessFor(courseId, CourseRole.Instructor))
				return Task.FromResult(new List<Group>());

			return GetAvailableForUserGroupsAsync(new List<string> { courseId }, user, onlyArchived);
		}

		public async Task<List<Group>> GetAvailableForUserGroupsAsync(List<string> coursesIds, ClaimsPrincipal user, bool onlyArchived=false)
		{
			var coursesWhereUserCanSeeAllGroups = await GetCoursesWhereUserCanSeeAllGroupsAsync(user, coursesIds).ConfigureAwait(false);
			var otherCourses = new HashSet<string>(coursesIds).Except(coursesWhereUserCanSeeAllGroups).ToList();

			var userId = user.GetUserId();
			var groupsWithAccess = new HashSet<int>(db.GroupAccesses.Where(a => a.UserId == userId && a.IsEnabled).Select(a => a.GroupId));
			var groups = db.Groups
				.Include(g => g.Owner)
				.Where(g => !g.IsDeleted && (onlyArchived ? g.IsArchived : !g.IsArchived) &&
							(
								/* Course admins can see all groups */
								coursesWhereUserCanSeeAllGroups.Contains(g.CourseId) ||
								/* Other instructor can see only own groups */
								(otherCourses.Contains(g.CourseId) && (g.OwnerId == userId || groupsWithAccess.Contains(g.Id)))
							)
			);

			return await groups
				.OrderBy(g => g.IsArchived)
				.ThenBy(g => g.OwnerId != userId)
				.ThenBy(g => g.Name)
				.ToListAsync()
				.ConfigureAwait(false);
		}
		
		/* Instructor can view student if he is a course admin or if student is member of one of accessible for instructor group */
		public async Task<bool> CanInstructorViewStudentAsync(ClaimsPrincipal instructor, string studentId)
		{
			if (instructor.HasAccess(CourseRole.CourseAdmin))
				return true;

			var coursesIds = courseManager.GetCourses().Select(c => c.Id).ToList();
			var groups = await GetAvailableForUserGroupsAsync(coursesIds, instructor).ConfigureAwait(false);
			var members = await groupMembersRepo.GetGroupsMembersAsync(groups.Select(g => g.Id).ToList()).ConfigureAwait(false);
			return members.Select(m => m.UserId).Contains(studentId);
		}

		public async Task<List<string>> GetCoursesWhereUserCanSeeAllGroupsAsync(ClaimsPrincipal user, IEnumerable<string> coursesIds)
		{
			var result = new List<string>();
			
			foreach (var courseId in coursesIds)
				if (await CanUserSeeAllCourseGroupsAsync(user, courseId).ConfigureAwait(false))
					result.Add(courseId);

			return result;
		}

		private async Task<bool> CanUserSeeAllCourseGroupsAsync(ClaimsPrincipal user, string courseId)
		{
			var userId = user.GetUserId();
			var canViewAllGroupMembersGlobal = await systemAccessesRepo.HasSystemAccessAsync(userId, SystemAccessType.ViewAllGroupMembers).ConfigureAwait(false);
			var canViewAllGroupMembersInCourse = await coursesRepo.HasCourseAccessAsync(userId, courseId, CourseAccessType.ViewAllGroupMembers).ConfigureAwait(false);
			return user.HasAccessFor(courseId, CourseRole.CourseAdmin) || canViewAllGroupMembersGlobal || canViewAllGroupMembersInCourse;
		}
	}
}