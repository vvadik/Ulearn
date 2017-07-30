using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using ApprovalUtilities.Utilities;
using Database;
using Database.DataContexts;
using Database.Extensions;
using Database.Models;
using Microsoft.AspNet.Identity;
using uLearn.Extensions;
using uLearn.Quizes;

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

		public static T GetFilterOptionsByGroup<T>(GroupsRepo groupsRepo, IPrincipal User, string courseId, List<string> groupsIds, bool allowSeeGroupForAnyMember = false) where T : AbstractFilterOptionByCourseAndUsers, new()
		{
			var result = new T { CourseId = courseId };

			/* if groupsIds contains "all" (it should be exclusive), get all users. Available only for instructors  */
			if (groupsIds.Contains("all") && User.HasAccessFor(courseId, CourseRole.Instructor))
				return result;
			/* if groupsIds contains "not-group" (it should be exclusive), get all users not in any groups, available only for course admins */
			if (groupsIds.Contains("not-in-group") && User.HasAccessFor(courseId, CourseRole.CourseAdmin))
			{
				var usersInGroups = groupsRepo.GetUsersIdsForAllGroups(courseId);
				result.UsersIds = usersInGroups.ToList();
				result.IsUserIdsSupplement = true;
				return result;
			}

			result.UsersIds = new List<string>();
			var usersIds = new HashSet<string>();

			/* if groupsIds is empty, get members of all own groups. Available for instructors */
			if ((groupsIds.Count == 0 || groupsIds.Any(string.IsNullOrEmpty)) && User.HasAccessFor(courseId, CourseRole.Instructor))
			{
				var ownGroupsIds = groupsRepo.GetGroupsOwnedByUser(courseId, User).Select(g => g.Id).ToList();
				foreach (var ownGroupId in ownGroupsIds)
				{
					var groupUsersIds = groupsRepo.GetGroupMembers(ownGroupId).Select(u => u.Id);
					usersIds.AddAll(groupUsersIds);
				}
				result.UsersIds = usersIds.ToList();
				return result;
			}

			foreach (var groupId in groupsIds)
			{
				int groupIdInt;
				if (int.TryParse(groupId, out groupIdInt))
				{
					var group = groupsRepo.FindGroupById(groupIdInt);
					if (group != null)
					{
						var groupMembersIds = groupsRepo.GetGroupMembers(group.Id).Select(u => u.Id).ToList();
						var hasAccessToGroup = groupsRepo.IsGroupAvailableForUser(group.Id, User);
						if (allowSeeGroupForAnyMember)
							hasAccessToGroup |= groupMembersIds.Contains(User.Identity.GetUserId());

						if (hasAccessToGroup)
							usersIds.AddAll(groupMembersIds);
					}
				}
			}
			result.UsersIds = usersIds.ToList();
			return result;
		}


		public static List<string> GetEnabledAdditionalScoringGroupsForGroups(GroupsRepo groupsRepo, Course course, List<string> groupsIds, IPrincipal User)
		{
			if (groupsIds.Contains("all") || groupsIds.Contains("not-in-group"))
				return course.Settings.Scoring.Groups.Keys.ToList();

			var enabledAdditionalScoringGroupsForGroups = groupsRepo.GetEnabledAdditionalScoringGroups(course.Id)
				.GroupBy(e => e.GroupId)
				.ToDictionary(g => g.Key, g => g.Select(e => e.ScoringGroupId).ToList());

			/* if groupsIds is empty, get members of all own groups. Available for instructors */
			if (groupsIds.Count == 0 || groupsIds.Any(string.IsNullOrEmpty))
			{
				var ownGroupsIds = groupsRepo.GetGroupsOwnedByUser(course.Id, User).Select(g => g.Id).ToList();
				return enabledAdditionalScoringGroupsForGroups.Where(kv => ownGroupsIds.Contains(kv.Key)).SelectMany(kv => kv.Value).ToList();
			}

			var result = new List<string>();
			foreach (var groupId in groupsIds)
			{
				int groupIdInt;
				if (int.TryParse(groupId, out groupIdInt))
					result.AddRange(enabledAdditionalScoringGroupsForGroups.GetOrDefault(groupIdInt, new List<string>()));
			}
			return result;
		}

		public static HashSet<Guid> GetSolvedSlides(UserSolutionsRepo solutionsRepo, UserQuizzesRepo userQuizzesRepo, Course course, string userId)
		{
			var solvedSlides = solutionsRepo.GetIdOfPassedSlides(course.Id, userId);
			solvedSlides.UnionWith(userQuizzesRepo.GetIdOfQuizPassedSlides(course.Id, userId));
			return solvedSlides;
		}

		public static int GetMaxScoreForUsersSlide(Slide slide, bool isSolved, bool hasManualChecking, bool enabledManualCheckingForUser)
		{
			var isExerciseOrQuiz = slide is ExerciseSlide || slide is QuizSlide;

			if (!isExerciseOrQuiz)
				return slide.MaxScore;

			if (isSolved)
				return hasManualChecking ? slide.MaxScore : GetMaxScoreWithoutManualChecking(slide);
			else
				return enabledManualCheckingForUser ? slide.MaxScore : GetMaxScoreWithoutManualChecking(slide);
		}

		private static int GetMaxScoreWithoutManualChecking(Slide slide)
		{
			if (slide is ExerciseSlide)
				return (slide as ExerciseSlide).Exercise.CorrectnessScore;
			if (slide is QuizSlide)
				return (slide as QuizSlide).ManualChecking ? 0 : slide.MaxScore;
			return slide.MaxScore;
		}
	}
}