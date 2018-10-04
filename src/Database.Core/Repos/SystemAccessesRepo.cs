using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Database.Extensions;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repos
{
	public class SystemAccessesRepo
	{
		private readonly UlearnDb db;

		public SystemAccessesRepo(UlearnDb db)
		{
			this.db = db;
		}
		
		public async Task<SystemAccess> GrantAccessAsync(string userId, SystemAccessType accessType, string grantedById)
		{
			var currentAccess = await db.SystemAccesses.FirstOrDefaultAsync(a => a.UserId == userId && a.AccessType == accessType).ConfigureAwait(false);
			if (currentAccess == null)
			{
				currentAccess = new SystemAccess
				{
					UserId = userId,
					AccessType = accessType,
				};
				db.SystemAccesses.Add(currentAccess);
			}
			currentAccess.GrantedById = grantedById;
			currentAccess.GrantTime = DateTime.Now;
			currentAccess.IsEnabled = true;

			await db.SaveChangesAsync().ConfigureAwait(false);
			return db.SystemAccesses.Include(a => a.GrantedBy).Single(a => a.Id == currentAccess.Id);
		}

		public bool CanRevokeAccess(string userId, IPrincipal revokedBy)
		{
			return revokedBy.IsSystemAdministrator();
		}

		public async Task<List<SystemAccess>> RevokeAccessAsync(string userId, SystemAccessType accessType)
		{
			var accesses = await db.SystemAccesses.Where(a => a.UserId == userId && a.AccessType == accessType).ToListAsync().ConfigureAwait(false);
			foreach (var access in accesses)
				access.IsEnabled = false;

			await db.SaveChangesAsync().ConfigureAwait(false);
			return accesses;
		}

		public Task<List<SystemAccess>> GetSystemAccessesAsync()
		{
			return db.SystemAccesses.Include(a => a.User).Where(a => a.IsEnabled).ToListAsync();
		}

		public Task<List<SystemAccess>> GetSystemAccessesAsync(string userId)
		{
			return db.SystemAccesses.Include(a => a.User).Where(a => a.UserId == userId && a.IsEnabled).ToListAsync();
		}

		public Task<bool> HasSystemAccessAsync(string userId, SystemAccessType accessType)
		{
			return db.SystemAccesses.AnyAsync(a => a.UserId == userId && a.AccessType == accessType && a.IsEnabled);
		}
	}
}