using System;
using System.Collections.Generic;
using System.Linq;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	public static class AppUserQueryExtentions
	{
		public static IQueryable<ApplicationUser> FilterByName(this IQueryable<ApplicationUser> applicationUsers, string namePrefix)
		{
			return String.IsNullOrEmpty(namePrefix)
				? applicationUsers
				: applicationUsers.Where(u => u.UserName.StartsWith(namePrefix));
		}

		public static IQueryable<ApplicationUser> FilterByRole(this IQueryable<ApplicationUser> applicationUsers, string role)
		{
			return String.IsNullOrEmpty(role)
				? applicationUsers
				: applicationUsers.Where(u => u.Roles.Any(r => r.Role.Name == role));
		}

		public static IQueryable<ApplicationUser> FilterByUserIds(this IQueryable<ApplicationUser> applicationUsers, params List<string>[] idLists)
		{
			HashSet<string> goodIds = null;
			foreach (var idList in idLists.Where(idList => idList != null))
			{
				if (goodIds == null)
					goodIds = new HashSet<string>(idList);
				else 
					goodIds.IntersectWith(idList);
			}

			return goodIds == null 
				? applicationUsers
				: applicationUsers.Where(user => goodIds.Contains(user.Id));
		}

		public static List<UserModel> GetUserModels(this IQueryable<ApplicationUser> applicationUsers, int count)
		{
			return applicationUsers
				.OrderBy(u => u.UserName)
				.Take(count)
				.Select(user => new UserModel
				{
					UserId = user.Id,
					UserName = user.UserName,
					GroupName = user.GroupName,
					Roles = user.Roles.Select(userRole => userRole.Role.Name).ToList()
				})
				.ToList();
		}
	}
}