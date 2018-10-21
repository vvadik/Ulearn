using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Database.Repos.CourseRoles;
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
		private readonly ICourseRolesRepo courseRolesRepo;
		private readonly WebCourseManager courseManager;
		private readonly ILogger logger;

		public GroupAccessesRepo(
			UlearnDb db,
			IGroupsRepo groupsRepo, ISystemAccessesRepo systemAccessesRepo, ICoursesRepo coursesRepo, IGroupMembersRepo groupMembersRepo,
			ICourseRolesRepo courseRolesRepo,
			WebCourseManager courseManager,
			ILogger logger
		)
		{
			this.db = db;
			this.groupsRepo = groupsRepo;
			this.systemAccessesRepo = systemAccessesRepo;
			this.coursesRepo = coursesRepo;
			this.groupMembersRepo = groupMembersRepo;
			this.courseRolesRepo = courseRolesRepo;
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

		/* TODO (andgein): move it to GroupController */
		public async Task<bool> CanRevokeAccessAsync(int groupId, string userId, string revokedById)
		{
			var group = await groupsRepo.FindGroupByIdAsync(groupId).ConfigureAwait(false) ?? throw new ArgumentNullException($"Can't find group with id={groupId}");
			var isCourseAdmin = await courseRolesRepo.HasUserAccessToCourseAsync(revokedById, @group.CourseId, CourseRoleType.CourseAdmin).ConfigureAwait(false);
			if (group.OwnerId == revokedById || isCourseAdmin)
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
		
		public async Task<bool> IsGroupVisibleForUserAsync(int groupId, string userId)
		{
			var group = await groupsRepo.FindGroupByIdAsync(groupId).ConfigureAwait(false) ?? throw new ArgumentNullException($"Can't find group with id={groupId}");
			return await IsGroupVisibleForUserAsync(group, userId).ConfigureAwait(false);
		}

		public async Task<bool> IsGroupVisibleForUserAsync(Group group, string userId)
		{
			/* Course admins and other privileged users can see all groups in the course */
			if (await CanUserSeeAllCourseGroupsAsync(userId, group.CourseId).ConfigureAwait(false))
				return true;

			if (! await courseRolesRepo.HasUserAccessToCourseAsync(userId, group.CourseId, CourseRoleType.Instructor).ConfigureAwait(false))
				return false;

			return await HasUserAccessToGroupAsync(group.Id, userId).ConfigureAwait(false);
		}

		public async Task<List<Group>> GetAvailableForUserGroupsAsync(string courseId, string userId, bool onlyArchived=false)
		{
			if (!await courseRolesRepo.HasUserAccessToCourseAsync(userId, courseId, CourseRoleType.Instructor).ConfigureAwait(false))
				return new List<Group>();

			return await GetAvailableForUserGroupsAsync(new List<string> { courseId }, userId, onlyArchived).ConfigureAwait(false);
		}

		public async Task<List<Group>> GetAvailableForUserGroupsAsync(List<string> coursesIds, string userId, bool onlyArchived=false)
		{
			var coursesWhereUserCanSeeAllGroups = await GetCoursesWhereUserCanSeeAllGroupsAsync(userId, coursesIds).ConfigureAwait(false);
			var otherCourses = new HashSet<string>(coursesIds).Except(coursesWhereUserCanSeeAllGroups).ToList();

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
		public async Task<bool> CanInstructorViewStudentAsync(string instructorId, string studentId)
		{
			if (await courseRolesRepo.HasUserAccessToAnyCourseAsync(instructorId, CourseRoleType.CourseAdmin).ConfigureAwait(false))
				return true;

			var coursesIds = courseManager.GetCourses().Select(c => c.Id).ToList();
			var groups = await GetAvailableForUserGroupsAsync(coursesIds, instructorId).ConfigureAwait(false);
			var members = await groupMembersRepo.GetGroupsMembersAsync(groups.Select(g => g.Id).ToList()).ConfigureAwait(false);
			return members.Select(m => m.UserId).Contains(studentId);
		}

		public async Task<List<string>> GetCoursesWhereUserCanSeeAllGroupsAsync(string userId, IEnumerable<string> coursesIds)
		{
			var result = new List<string>();
			
			foreach (var courseId in coursesIds)
				if (await CanUserSeeAllCourseGroupsAsync(userId, courseId).ConfigureAwait(false))
					result.Add(courseId);

			return result;
		}

		private async Task<bool> CanUserSeeAllCourseGroupsAsync(string userId, string courseId)
		{
			var canViewAllGroupMembersGlobal = await systemAccessesRepo.HasSystemAccessAsync(userId, SystemAccessType.ViewAllGroupMembers).ConfigureAwait(false);
			var canViewAllGroupMembersInCourse = await coursesRepo.HasCourseAccessAsync(userId, courseId, CourseAccessType.ViewAllGroupMembers).ConfigureAwait(false);
			var isCourseAdmin = await courseRolesRepo.HasUserAccessToCourseAsync(userId, courseId, CourseRoleType.CourseAdmin).ConfigureAwait(false);
			return isCourseAdmin || canViewAllGroupMembersGlobal || canViewAllGroupMembersInCourse;
		}
	}
}