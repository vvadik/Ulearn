using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using uLearn.Web.DataContexts;
using uLearn.Web.Extensions;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public static class ControllerUtils
	{
		public static bool HasPassword(UserManager<ApplicationUser> userManager, IPrincipal principal)
		{
			var user = userManager.FindById(principal.Identity.GetUserId());
			return user?.PasswordHash != null;
		}

		public static string FixRedirectUrl(this Controller controller, string returnUrl)
		{
			return controller.Url.IsLocalUrl(returnUrl) ? returnUrl : controller.Url.Action("Index", "Home");
		}

		public static void AddErrors(this Controller controller, IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				controller.ModelState.AddModelError("", error);
			}
		}

		public static T GetFilterOptionsByGroup<T>(GroupsRepo groupsRepo, IPrincipal User, string courseId, string groupId) where T : AbstractFilterOptionByCourseAndUsers, new()
		{
			var result = new T { CourseId = courseId };

			/* if groupId = "all", get all users */
			if (groupId == "all")
				return result;
			/* if groupId = "not-group", get all users not in any groups, available only for course admins */
			if (groupId == "not-in-group" && User.HasAccessFor(courseId, CourseRole.CourseAdmin))
			{
				var usersInGroups = groupsRepo.GetUsersIdsForAllGroups(courseId);
				result.UsersIds = usersInGroups.ToList();
				result.IsUserIdsSupplement = true;
				return result;
			}

			/* if groupId is null, get memers of all own groups */
			if (string.IsNullOrEmpty(groupId))
			{
				var ownGroupsIds = groupsRepo.GetGroupsOwnedByUser(courseId, User).Select(g => g.Id).ToList();
				var usersIds = new List<string>();
				foreach (var ownGroupId in ownGroupsIds)
				{
					var groupUsersIds = groupsRepo.GetGroupMembers(ownGroupId).Select(u => u.Id).ToList();
					usersIds.AddRange(groupUsersIds);
				}
				result.UsersIds = usersIds;
				return result;
			}

			int groupIdInt;
			if (int.TryParse(groupId, out groupIdInt))
			{
				var group = groupsRepo.FindGroupById(groupIdInt);
				if (group != null && groupsRepo.IsGroupAvailableForUser(group.Id, User))
				{
					var usersIds = groupsRepo.GetGroupMembers(group.Id).Select(u => u.Id);
					result.UsersIds = usersIds.ToList();
					return result;
				}
			}
			return result;
		}
	}
}