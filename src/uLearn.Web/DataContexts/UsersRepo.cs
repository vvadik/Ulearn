using System.Collections.Generic;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class UsersRepo
	{
		private readonly ULearnDb db;
		private UserRolesRepo userRolesRepo;

		public UsersRepo() : this(new ULearnDb())
		{
			
		}

		public UsersRepo(ULearnDb db)
		{
			this.db = db;
			userRolesRepo = new UserRolesRepo(db);
		}

		public List<UserRolesInfo> FilterUsers(UserSearchQueryModel query)
		{
			return db.Users
				.FilterByName(query.NamePrefix)
				.FilterByRole(query.Role)
				.FilterByUserIds(
					userRolesRepo.GetListOfUsersWithCourseRole(query.CourseRole, query.CourseId),
					userRolesRepo.GetListOfUsersByPrivilege(query.OnlyPrivileged, query.CourseId)
				)
				.GetUserRolesInfo(50);
		}
 
	}
}