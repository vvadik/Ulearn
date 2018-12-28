using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos
{
	public interface ISystemAccessesRepo
	{
		Task<SystemAccess> GrantAccessAsync(string userId, SystemAccessType accessType, string grantedById);
		bool CanRevokeAccess(string userId, IPrincipal revokedBy);
		Task<List<SystemAccess>> RevokeAccessAsync(string userId, SystemAccessType accessType);
		Task<List<SystemAccess>> GetSystemAccessesAsync();
		Task<List<SystemAccess>> GetSystemAccessesAsync(string userId);
		Task<bool> HasSystemAccessAsync(string userId, SystemAccessType accessType);
	}
}