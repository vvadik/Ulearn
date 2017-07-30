using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Database.Extensions;
using Database.Models;
using log4net;
using Microsoft.AspNet.Identity;
using uLearn;
using uLearn.Extensions;
using uLearn.Quizes;

namespace Database.DataContexts
{
	public class GroupsRepo
	{
		private readonly ILog log = LogManager.GetLogger(typeof(GroupsRepo));

		private readonly ULearnDb db;
		private readonly SlideCheckingsRepo slideCheckingsRepo;
		private readonly UserSolutionsRepo userSolutionsRepo;
		private readonly UserQuizzesRepo userQuizzesRepo;
		private readonly VisitsRepo visitsRepo;

		private readonly CourseManager courseManager;

		public GroupsRepo(ULearnDb db, CourseManager courseManager)
		{
			this.db = db;
			this.courseManager = courseManager;
			slideCheckingsRepo = new SlideCheckingsRepo(db);
			userSolutionsRepo = new UserSolutionsRepo(db, courseManager);
			userQuizzesRepo = new UserQuizzesRepo(db);
			visitsRepo = new VisitsRepo(db);
		}

		public bool CanUserSeeAllCourseGroups(IPrincipal user, string courseId)
		{
			return user.HasAccessFor(courseId, CourseRole.CourseAdmin);
		}

		public async Task<Group> CreateGroup(string courseId, string name, string ownerId, bool isPublic = false, bool isManualCheckingEnabled = false, bool isManualCheckingEnabledForOldSolutions = false)
		{
			var group = new Group
			{
				CourseId = courseId,
				Name = name,
				IsPublic = isPublic,
				OwnerId = ownerId,
				IsManualCheckingEnabled = isManualCheckingEnabled,
				IsManualCheckingEnabledForOldSolutions = isManualCheckingEnabledForOldSolutions,
			};
			db.Groups.Add(group);
			await db.SaveChangesAsync();

			return group;
		}

		/* Copy group from one course to another. Replaces owner only if newOwnerId is not empty */

		public async Task<Group> CopyGroup(Group group, string courseId, string newOwnerId = "")
		{
			var newGroup = await CopyGroupWithoutMembers(group, courseId, newOwnerId);
			await CopyGroupMembers(group, newGroup);
			return newGroup;
		}

		private async Task<Group> CopyGroupWithoutMembers(Group group, string courseId, string newOwnerId)
		{
			var newGroup = new Group
			{
				CourseId = courseId,
				OwnerId = string.IsNullOrEmpty(newOwnerId) ? group.OwnerId : newOwnerId,
				Name = group.Name,
				CanUsersSeeGroupProgress = group.CanUsersSeeGroupProgress,
				IsManualCheckingEnabled = group.IsManualCheckingEnabled,
				IsInviteLinkEnabled = group.IsInviteLinkEnabled,
				IsPublic = group.IsPublic,
				InviteHash = Guid.NewGuid(),
			};
			db.Groups.Add(newGroup);
			await db.SaveChangesAsync();
			return newGroup;
		}

		private async Task CopyGroupMembers(Group group, Group newGroup)
		{
			var members = @group.Members.Select(m => new GroupMember
			{
				UserId = m.UserId,
				GroupId = newGroup.Id,
			});
			db.GroupMembers.AddRange(members);
			await db.SaveChangesAsync();
		}

		public async Task<Group> ModifyGroup(int groupId, string newName, bool newIsPublic, bool newIsManualCheckingEnabled, bool newIsManualCheckingEnabledForOldSolutions, bool newIsArchived, string newOwnerId)
		{
			var group = FindGroupById(groupId);
			group.Name = newName;
			group.IsPublic = newIsPublic;
			group.IsManualCheckingEnabled = newIsManualCheckingEnabled;

			if (!group.IsManualCheckingEnabledForOldSolutions && newIsManualCheckingEnabledForOldSolutions)
				await AddManualCheckingsForOldSolutions(group.CourseId, group.Members.Select(m => m.UserId).ToList());

			group.IsManualCheckingEnabledForOldSolutions = newIsManualCheckingEnabledForOldSolutions;
			group.IsArchived = newIsArchived;
			group.OwnerId = newOwnerId;
			await db.SaveChangesAsync();

			return group;
		}

		public async Task RemoveGroup(int groupId)
		{
			var group = db.Groups.Find(groupId);
			group.IsDeleted = true;
			await db.SaveChangesAsync();
		}

		public async Task<bool> AddUserToGroup(int groupId, string userId)
		{
			var groupMember = new GroupMember
			{
				GroupId = groupId,
				UserId = userId
			};
			using (var transaction = db.Database.BeginTransaction())
			{
				/* Don't add member if it's already exists */
				var existsMember = db.GroupMembers.FirstOrDefault(m => m.GroupId == groupId && m.UserId == userId);
				if (existsMember != null)
					return false;

				db.GroupMembers.Add(groupMember);
				await db.SaveChangesAsync();

				transaction.Commit();
			}

			var group = FindGroupById(groupId);
			if (group != null && group.IsManualCheckingEnabledForOldSolutions)
				await AddManualCheckingsForOldSolutions(group.CourseId, userId);

			return true;
		}

		private async Task AddManualCheckingsForOldSolutions(string courseId, IEnumerable<string> usersIds)
		{
			foreach (var userId in usersIds)
				await AddManualCheckingsForOldSolutions(courseId, userId);
		}

		private async Task AddManualCheckingsForOldSolutions(string courseId, string userId)
		{
			log.Info($"Создаю ручные проверки для всех решения пользователя {userId} в курсе {courseId}");

			var course = courseManager.GetCourse(courseId);

			/* For exercises */
			var acceptedSubmissionsBySlide = userSolutionsRepo.GetAllAcceptedSubmissionsByUser(courseId, userId)
				.GroupBy(s => s.SlideId)
				.ToDictionary(g => g.Key, g => g.ToList());
			foreach (var acceptedSubmissionsForSlide in acceptedSubmissionsBySlide.Values)
				/* If exists at least one manual checking for at least one submissions on slide, then ignore this slide */
				if (!acceptedSubmissionsForSlide.Any(s => s.ManualCheckings.Any()))
				{
					/* Otherwise found the latest accepted submission */
					var lastSubmission = acceptedSubmissionsForSlide.OrderByDescending(s => s.Timestamp).First();

					var slideId = lastSubmission.SlideId;
					var slide = course.FindSlideById(slideId) as ExerciseSlide;
					if (slide == null || !slide.Exercise.RequireReview)
						continue;

					log.Info($"Создаю ручную проверку для решения {lastSubmission.Id}, слайд {slideId}");
					await slideCheckingsRepo.AddManualExerciseChecking(courseId, slideId, userId, lastSubmission);
					await visitsRepo.MarkVisitsAsWithManualChecking(slideId, userId);
				}

			/* For quizzes */
			var passedQuizzesIds = userQuizzesRepo.GetIdOfQuizPassedSlides(courseId, userId);
			foreach (var quizSlideId in passedQuizzesIds)
			{
				var slide = course.FindSlideById(quizSlideId) as QuizSlide;
				if (slide == null || !slide.ManualChecking)
					continue;
				if (!userQuizzesRepo.IsWaitingForManualCheck(courseId, quizSlideId, userId))
				{
					log.Info($"Создаю ручную проверку для теста {slide.Id}");
					await slideCheckingsRepo.AddQuizAttemptForManualChecking(courseId, quizSlideId, userId);
					await visitsRepo.MarkVisitsAsWithManualChecking(quizSlideId, userId);
				}
			}
		}

		public async Task<bool> AddUserToGroup(int groupId, ApplicationUser user)
		{
			return await AddUserToGroup(groupId, user.Id);
		}

		public async Task RemoveUserFromGroup(int groupId, string userId)
		{
			var member = db.GroupMembers.FirstOrDefault(m => m.GroupId == groupId && m.UserId == userId);
			if (member != null)
				db.GroupMembers.Remove(member);
			await db.SaveChangesAsync();
		}

		public Group FindGroupById(int groupId)
		{
			return db.Groups.FirstOrDefault(g => g.Id == groupId && !g.IsDeleted);
		}

		public Group FindGroupByInviteHash(Guid hash)
		{
			return db.Groups.FirstOrDefault(g => g.InviteHash == hash && !g.IsDeleted && g.IsInviteLinkEnabled);
		}

		public IEnumerable<Group> GetGroups(string courseId, bool includeArchived = false)
		{
			var groups = db.Groups.Where(g => g.CourseId == courseId && !g.IsDeleted);
			if (!includeArchived)
				groups = groups.Where(g => !g.IsArchived);
			return groups;
		}

		public bool IsGroupAvailableForUser(int groupId, IPrincipal user)
		{
			var group = FindGroupById(groupId);
			/* Course admins can see all groups */
			if (CanUserSeeAllCourseGroups(user, group.CourseId))
				return true;

			if (!user.HasAccessFor(group.CourseId, CourseRole.Instructor))
				return false;

			var userId = user.Identity.GetUserId();
			return !group.IsDeleted && (group.OwnerId == userId || group.IsPublic);
		}

		public List<Group> GetAvailableForUserGroups(string courseId, IPrincipal user, bool includeArchived = false)
		{
			if (!user.HasAccessFor(courseId, CourseRole.Instructor))
				return new List<Group>();

			return GetAvailableForUserGroups(new List<string> { courseId }, user, includeArchived);
		}

		public List<Group> GetAvailableForUserGroups(List<string> coursesIds, IPrincipal user, bool includeArchived = false)
		{
			var coursesWhereUserCanSeeAllGroups = coursesIds.Where(id => CanUserSeeAllCourseGroups(user, id)).ToList();
			var otherCourses = new HashSet<string>(coursesIds).Except(coursesWhereUserCanSeeAllGroups).ToList();

			var userId = user.Identity.GetUserId();
			var groups = db.Groups.Where(g => !g.IsDeleted && (includeArchived || !g.IsArchived) &&
											(
												/* Course admins can see all groups */
												coursesWhereUserCanSeeAllGroups.Contains(g.CourseId) ||
												/* Other instructor can see only public or own groups */
												(otherCourses.Contains(g.CourseId) && (g.OwnerId == userId || g.IsPublic))
											)
			);

			return groups
				.OrderBy(g => g.IsArchived)
				.ThenBy(g => g.OwnerId != userId)
				.ThenBy(g => g.Name)
				.ToList();
		}

		public List<Group> GetGroupsOwnedByUser(string courseId, IPrincipal user, bool includeArchived = false)
		{
			var userId = user.Identity.GetUserId();
			var groups = db.Groups.Where(g => g.CourseId == courseId && !g.IsDeleted && g.OwnerId == userId);
			if (!includeArchived)
				groups = groups.Where(g => !g.IsArchived);
			return groups.ToList();
		}

		public List<ApplicationUser> GetGroupMembers(int groupId)
		{
			return db.GroupMembers.Where(m => m.GroupId == groupId).Select(m => m.User).ToList();
		}

		public Dictionary<string, List<Group>> GetUsersGroups(List<string> courseIds, IEnumerable<string> userIds, IPrincipal currentUser, int maxCount = 3)
		{
			var canSeeAllGroups = courseIds.ToDictionary(c => c.ToLower(), c => CanUserSeeAllCourseGroups(currentUser, c));
			var currentUserId = currentUser.Identity.GetUserId();

			var usersGroups = db.GroupMembers
				.Where(m => userIds.Contains(m.UserId) && courseIds.Contains(m.Group.CourseId))
				.GroupBy(m => m.UserId)
				.ToDictionary(group => group.Key, group => group.ToList())
				.ToDictionary(
					kv => kv.Key,
					kv => kv.Value.Select(m => m.Group)
						.Distinct()
						.Where(g => (g.OwnerId == currentUserId || g.IsPublic || canSeeAllGroups[g.CourseId.ToLower()]) && !g.IsDeleted && !g.IsArchived)
						.OrderBy(g => g.OwnerId != currentUserId)
						.Take(maxCount)
						.ToList()
				);

			return usersGroups;
		}

		public Dictionary<string, List<string>> GetUsersGroupsNames(List<string> courseIds, IEnumerable<string> userIds, IPrincipal currentUser, int maxCount = 3)
		{
			var usersGroups = GetUsersGroups(courseIds, userIds, currentUser, maxCount + 1);
			return usersGroups.ToDictionary(
				kv => kv.Key,
				kv => kv.Value.Select((g, idx) => idx >= maxCount ? "..." : g.Name).ToList());
		}

		public Dictionary<string, List<int>> GetUsersGroupsIds(List<string> courseIds, IEnumerable<string> userIds, IPrincipal currentUser, int maxCount = 3)
		{
			var usersGroups = GetUsersGroups(courseIds, userIds, currentUser, maxCount);
			return usersGroups.ToDictionary(
				kv => kv.Key,
				kv => kv.Value.Select(g => g.Id).ToList());
		}

		public Dictionary<string, string> GetUsersGroupsNamesAsStrings(List<string> courseIds, IEnumerable<string> userIds, IPrincipal currentUser, int maxCount = 3)
		{
			var usersGroups = GetUsersGroupsNames(courseIds, userIds, currentUser, maxCount);
			return usersGroups.ToDictionary(kv => kv.Key, kv => string.Join(", ", kv.Value));
		}

		public Dictionary<string, string> GetUsersGroupsNamesAsStrings(string courseId, IEnumerable<string> userIds, IPrincipal currentUser, int maxCount = 3)
		{
			return GetUsersGroupsNamesAsStrings(new List<string> { courseId }, userIds, currentUser, maxCount);
		}

		public string GetUserGroupsNamesAsString(List<string> courseIds, string userId, IPrincipal currentUser, int maxCount = 3)
		{
			var usersGroups = GetUsersGroupsNamesAsStrings(courseIds, new List<string> { userId }, currentUser, maxCount);
			return usersGroups.GetOrDefault(userId, "");
		}

		public string GetUserGroupsNamesAsString(string courseId, string userId, IPrincipal currentUser, int maxCount = 3)
		{
			return GetUserGroupsNamesAsString(new List<string> { courseId }, userId, currentUser, maxCount);
		}

		public async Task EnableGroupInviteLink(int groupId, bool isEnabled)
		{
			var group = db.Groups.Find(groupId);
			group.IsInviteLinkEnabled = isEnabled;
			await db.SaveChangesAsync();
		}

		public IEnumerable<string> GetUsersIdsForAllGroups(string courseId)
		{
			var groupsIds = GetGroups(courseId).Select(g => g.Id);
			return db.GroupMembers.Where(m => groupsIds.Contains(m.GroupId)).Select(m => m.UserId);
		}

		public Dictionary<string, List<int>> GetUsersGroupsIds(string courseId, IEnumerable<string> usersIds)
		{
			var groupsIds = GetGroups(courseId).Select(g => g.Id);
			return db.GroupMembers
				.Where(m => groupsIds.Contains(m.GroupId) && usersIds.Contains(m.UserId))
				.GroupBy(m => m.UserId)
				.ToDictionary(g => g.Key, g => g.Select(m => m.GroupId).ToList());
		}

		public List<int> GetUserGroupsIds(string courseId, string userId)
		{
			return GetUsersGroupsIds(courseId, new List<string> { userId }).GetOrDefault(userId, new List<int>());
		}

		public List<Group> GetUserGroups(string courseId, string userId)
		{
			var userGroupsIds = GetUserGroupsIds(courseId, userId);
			return GetGroups(courseId).Where(g => userGroupsIds.Contains(g.Id)).ToList();
		}

		public bool IsManualCheckingEnabledForUser(Course course, string userId)
		{
			if (course.Settings.IsManualCheckingEnabled)
				return true;

			var userGroupsIds = db.GroupMembers
				.Where(m => m.Group.CourseId == course.Id && m.UserId == userId && !m.Group.IsDeleted)
				.DistinctBy(m => m.GroupId)
				.Select(m => m.GroupId)
				.ToList();
			var userGroups = db.Groups.Where(g => userGroupsIds.Contains(g.Id)).ToList();
			return userGroups.Any(g => g.IsManualCheckingEnabled);
		}

		public async Task EnableAdditionalScoringGroupsForGroup(int groupId, IEnumerable<string> scoringGroupsIds)
		{
			using (var transaction = db.Database.BeginTransaction())
			{
				db.EnabledAdditionalScoringGroups.RemoveRange(
					db.EnabledAdditionalScoringGroups.Where(e => e.GroupId == groupId)
				);

				foreach (var scoringGroupId in scoringGroupsIds)
					db.EnabledAdditionalScoringGroups.Add(new EnabledAdditionalScoringGroup
					{
						GroupId = groupId,
						ScoringGroupId = scoringGroupId
					});

				await db.SaveChangesAsync();

				transaction.Commit();
			}
		}

		public List<EnabledAdditionalScoringGroup> GetEnabledAdditionalScoringGroups(string courseId)
		{
			var groupsIds = GetGroups(courseId).Select(g => g.Id);
			return db.EnabledAdditionalScoringGroups.Where(e => groupsIds.Contains(e.GroupId)).ToList();
		}
	}
}