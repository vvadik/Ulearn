using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Database.Extensions;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Ulearn.Common;
using Ulearn.Common.Extensions;

namespace Database.Repos.Groups
{
	public class GroupAccessesRepo
	{
		private readonly UlearnDb db;
		private readonly GroupsRepo groupsRepo;

		public GroupAccessesRepo(UlearnDb db, GroupsRepo groupsRepo)
		{
			this.db = db;
			this.groupsRepo = groupsRepo;
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
			return db.GroupAccesses.Include(a => a.User).Where(a => a.GroupId == groupId && a.IsEnabled && !a.User.IsDeleted).ToListAsync();
		}

		public async Task<DefaultDictionary<int, List<GroupAccess>>> GetGroupAccessesAsync(IEnumerable<int> groupsIds)
		{
			var groupAccesses = await db.GroupAccesses
				.Include(a => a.User)
				.Where(a => groupsIds.Contains(a.GroupId) && a.IsEnabled && !a.User.IsDeleted)
				.GroupBy(a => a.GroupId)
				.ToDictionaryAsync(g => g.Key, g => g.ToList())
				.ConfigureAwait(false);
			
			return groupAccesses.ToDefaultDictionary();
		}
	}
}