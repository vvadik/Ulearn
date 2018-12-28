using System.Linq;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos.Users.Search
{
	public interface IAccessRestrictor
	{
		Task<IQueryable<ApplicationUser>> RestrictUsersSetAsync(IQueryable<ApplicationUser> users, ApplicationUser currentUser,
			bool hasSystemAdministratorAccess, bool hasCourseAdminAccess, bool hasInstructorAccessToGroupMembers, bool hasInstructorAccessToGroupInstructors);
	}
}