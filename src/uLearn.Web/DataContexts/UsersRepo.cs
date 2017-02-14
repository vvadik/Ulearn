using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Objects;
using System.Linq;
using EntityFramework.Functions;
using Microsoft.AspNet.Identity;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public class UsersRepo
	{
		private readonly ULearnDb db;
		private readonly UserRolesRepo userRolesRepo;

		public UsersRepo(ULearnDb db)
		{
			this.db = db;
			userRolesRepo = new UserRolesRepo(db);
		}

		public ApplicationUser FindUserById(string id)
		{
			return db.Users.Find(id);
		}

		public List<UserRolesInfo> FilterUsers(UserSearchQueryModel query, UserManager<ApplicationUser> userManager, int limit=50)
		{
			var role = db.Roles.FirstOrDefault(r => r.Name == query.Role);
			IQueryable<ApplicationUser> users = db.Users;
			if (!string.IsNullOrEmpty(query.NamePrefix))
			{
				var usersIds = GetUsersByNamePrefix(query.NamePrefix).Select(u => u.Id);
				users = users.Where(u => usersIds.Contains(u.Id));
			}
			return users
				.FilterByRole(role, userManager)
				.FilterByUserIds(
					userRolesRepo.GetListOfUsersWithCourseRole(query.CourseRole, query.CourseId),
					userRolesRepo.GetListOfUsersByPrivilege(query.OnlyPrivileged, query.CourseId)
				)
				.GetUserRolesInfo(limit, userManager);
		}

		public List<UserRolesInfo> GetCourseInstructors(string courseId, UserManager<ApplicationUser> userManager, int limit=50)
		{
			return db.Users
				.FilterByUserIds(userRolesRepo.GetListOfUsersWithCourseRole(CourseRole.Instructor, courseId, includeHighRoles: true))
				.GetUserRolesInfo(limit, userManager);
		}

		private const string nameSpace = nameof(UsersRepo);
		private const string dbo = nameof(dbo);

		[TableValuedFunction(nameof(GetUsersByNamePrefix), nameSpace, Schema = dbo)]
		// ReSharper disable once MemberCanBePrivate.Global
		public IQueryable<UserIdWrapper> GetUsersByNamePrefix(string name)
		{
			if (string.IsNullOrEmpty(name))
				return db.Users.Select(u => new UserIdWrapper(u.Id)); ;
			var splittedName = name.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			var nameQuery = string.Join(" & ", splittedName.Select(s => "\"" + s.Trim().Replace("\"", "\\\"") + "*\""));
			var nameParameter = new ObjectParameter("name", nameQuery);
			return db.ObjectContext().CreateQuery<UserIdWrapper>($"[{nameof(GetUsersByNamePrefix)}](@name)", nameParameter);
		}
	}

	/* System.String is not available for table-valued functions so we need to create ComplexTyped wrapper */
	[ComplexType]
	public class UserIdWrapper
	{
		public UserIdWrapper(string userId)
		{
			Id = userId;
		}

		public string Id { get; set; }
	}
}