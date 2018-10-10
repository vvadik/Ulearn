using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Database.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Serilog;
using uLearn;
using uLearn.Quizes;
using Ulearn.Common.Extensions;

namespace Database.Repos.Groups
{
	/* This repo is fully migrated to .NET Core and EF Core */
	public class GroupsRepo : IGroupsRepo
	{
		private readonly UlearnDb db;
		private readonly ISlideCheckingsRepo slideCheckingsRepo;
		private readonly IUserSolutionsRepo userSolutionsRepo;
		private readonly IUserQuizzesRepo userQuizzesRepo;
		private readonly IVisitsRepo visitsRepo;
		private readonly IGroupsCreatorAndCopier groupsCreatorAndCopier;

		private readonly WebCourseManager courseManager;
		
		private readonly ILogger logger;

		public GroupsRepo(
			UlearnDb db,
			ISlideCheckingsRepo slideCheckingsRepo, IUserSolutionsRepo userSolutionsRepo, IUserQuizzesRepo userQuizzesRepo, IVisitsRepo visitsRepo,
			IGroupsCreatorAndCopier groupsCreatorAndCopier,
			WebCourseManager courseManager,
			ILogger logger)
		{
			this.db = db;
			this.slideCheckingsRepo = slideCheckingsRepo;
			this.userSolutionsRepo = userSolutionsRepo;
			this.userQuizzesRepo = userQuizzesRepo;
			this.visitsRepo = visitsRepo;
			this.groupsCreatorAndCopier = groupsCreatorAndCopier;
			this.courseManager = courseManager;
			this.logger = logger;
		}

		public Task<Group> CreateGroupAsync(
			string courseId,
			string name,
			string ownerId,
			bool isManualCheckingEnabled=false,
			bool isManualCheckingEnabledForOldSolutions=false,
			bool canUsersSeeGroupProgress=true,
			bool defaultProhibitFurtherReview=true,
			bool isInviteLinkEnabled=true)
		{
			return groupsCreatorAndCopier.CreateGroupAsync(
				courseId,
				name,
				ownerId,
				isManualCheckingEnabled,
				isManualCheckingEnabledForOldSolutions,
				canUsersSeeGroupProgress,
				defaultProhibitFurtherReview,
				isInviteLinkEnabled
			);
		}

		/* Copy group from one course to another. Replace owner only if newOwnerId is not empty */
		public Task<Group> CopyGroupAsync(Group group, string courseId, string newOwnerId = "")
		{
			return groupsCreatorAndCopier.CopyGroupAsync(group, courseId, newOwnerId);
		}

		public async Task<Group> ModifyGroupAsync(
			int groupId,
			string newName,
			bool newIsManualCheckingEnabled,
			bool newIsManualCheckingEnabledForOldSolutions,
			bool newDefaultProhibitFurtherReview,
			bool newCanUsersSeeGroupProgress)
		{
			var group = await FindGroupByIdAsync(groupId).ConfigureAwait(false) ?? throw new ArgumentNullException($"Can't find group with id={groupId}");
				
			group.Name = newName;
			group.IsManualCheckingEnabled = newIsManualCheckingEnabled;

			if (!group.IsManualCheckingEnabledForOldSolutions && newIsManualCheckingEnabledForOldSolutions)
			{
				var groupMembers = group.NotDeletedMembers.Select(m => m.UserId).ToList();
				await AddManualCheckingsForOldSolutionsAsync(group.CourseId, groupMembers).ConfigureAwait(false);
			}

			group.IsManualCheckingEnabledForOldSolutions = newIsManualCheckingEnabledForOldSolutions;
			group.DefaultProhibitFutherReview = newDefaultProhibitFurtherReview;
			group.CanUsersSeeGroupProgress = newCanUsersSeeGroupProgress;
			
			await db.SaveChangesAsync().ConfigureAwait(false);

			return group;
		}

		public async Task ChangeGroupOwnerAsync(int groupId, string newOwnerId)
		{
			var group = await FindGroupByIdAsync(groupId).ConfigureAwait(false) ?? throw new ArgumentNullException($"Can't find group with id={groupId}");
			group.OwnerId = newOwnerId;

			await db.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task<Group> ArchiveGroupAsync(int groupId, bool isArchived)
		{
			var group = await FindGroupByIdAsync(groupId).ConfigureAwait(false) ?? throw new ArgumentNullException($"Can't find group with id={groupId}");
			group.IsArchived = isArchived;
			
			await db.SaveChangesAsync().ConfigureAwait(false);
			return group;
		}

		public async Task DeleteGroupAsync(int groupId)
		{
			var group = await FindGroupByIdAsync(groupId).ConfigureAwait(false);
			
			/* Maybe group is already deleted */
			if (group == null)
				return;
			
			group.IsDeleted = true;
			
			await db.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task<GroupMember> AddUserToGroupAsync(int groupId, string userId)
		{
			var group = await FindGroupByIdAsync(groupId).ConfigureAwait(false) ?? throw new ArgumentNullException($"Can't find group with id={groupId}");
			
			var groupMember = new GroupMember
			{
				GroupId = groupId,
				UserId = userId,
				AddingTime = DateTime.Now,
			};
			using (var transaction = db.Database.BeginTransaction())
			{
				/* Don't add member if it's already exists */
				var existsMember = db.GroupMembers.FirstOrDefault(m => m.GroupId == groupId && m.UserId == userId);
				if (existsMember != null)
					return null;

				db.GroupMembers.Add(groupMember);
				await db.SaveChangesAsync().ConfigureAwait(false);

				transaction.Commit();
			}
			
			if (group.IsManualCheckingEnabledForOldSolutions)
				await AddManualCheckingsForOldSolutionsAsync(group.CourseId, userId).ConfigureAwait(false);

			return groupMember;
		}

		public Task<GroupMember> AddUserToGroupAsync(int groupId, ApplicationUser user)
		{
			return AddUserToGroupAsync(groupId, user.Id);
		}

		private async Task AddManualCheckingsForOldSolutionsAsync(string courseId, IEnumerable<string> usersIds)
		{
			foreach (var userId in usersIds)
				await AddManualCheckingsForOldSolutionsAsync(courseId, userId).ConfigureAwait(false);
		}

		private async Task AddManualCheckingsForOldSolutionsAsync(string courseId, string userId)
		{
			logger.Information($"Создаю ручные проверки для всех решения пользователя {userId} в курсе {courseId}");

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

					logger.Information($"Создаю ручную проверку для решения {lastSubmission.Id}, слайд {slideId}");
					await slideCheckingsRepo.AddManualExerciseChecking(courseId, slideId, userId, lastSubmission).ConfigureAwait(false);
					await visitsRepo.MarkVisitsAsWithManualChecking(slideId, userId).ConfigureAwait(false);
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
					logger.Information($"Создаю ручную проверку для теста {slide.Id}");
					await slideCheckingsRepo.AddQuizAttemptForManualChecking(courseId, quizSlideId, userId).ConfigureAwait(false);
					await visitsRepo.MarkVisitsAsWithManualChecking(quizSlideId, userId).ConfigureAwait(false);
				}
			}
		}

		public async Task<GroupMember> RemoveUserFromGroupAsync(int groupId, string userId)
		{
			var member = db.GroupMembers.FirstOrDefault(m => m.GroupId == groupId && m.UserId == userId);
			if (member != null)
				db.GroupMembers.Remove(member);

			await db.SaveChangesAsync().ConfigureAwait(false);
			
			return member;
		}
		
		public async Task<List<GroupMember>> RemoveUsersFromGroupAsync(int groupId, List<string> userIds)
		{
			var members = db.GroupMembers.Where(m => m.GroupId == groupId && userIds.Contains(m.UserId)).ToList();
			db.GroupMembers.RemoveRange(members);

			await db.SaveChangesAsync().ConfigureAwait(false);
			
			return members;
		}
		
		public async Task<List<GroupMember>> CopyUsersFromOneGroupToAnotherAsync(int fromGroupId, int toGroupId, List<string> userIds)
		{
			var membersUserIds = db.GroupMembers.Where(m => m.GroupId == fromGroupId && userIds.Contains(m.UserId)).Select(m => m.UserId).ToList();
			var newMembers = new List<GroupMember>();
			foreach (var memberUserId in membersUserIds)
				newMembers.Add(await AddUserToGroupAsync(toGroupId, memberUserId).ConfigureAwait(false));
			
			return newMembers;
		}

		[ItemCanBeNull]
		public Task<Group> FindGroupByIdAsync(int groupId, bool noTracking=false)
		{
			var groups = db.Groups.AsQueryable();
			if (noTracking)
				groups = groups.AsNoTracking();
			return groups.FirstOrDefaultAsync(g => g.Id == groupId && !g.IsDeleted);
		}

		[ItemCanBeNull]
		public Task<Group> FindGroupByInviteHashAsync(Guid hash, bool noTracking=false)
		{
			var groups = db.Groups.AsQueryable();
			if (noTracking)
				groups = groups.AsNoTracking();
			return groups.FirstOrDefaultAsync(g => g.InviteHash == hash && !g.IsDeleted && g.IsInviteLinkEnabled);
		}

		private IQueryable<Group> GetCourseGroupsQueryable(string courseId, bool includeArchived=false)
		{
			var groups = db.Groups.Where(g => g.CourseId == courseId && !g.IsDeleted);
			if (!includeArchived)
				groups = groups.Where(g => !g.IsArchived);
			return groups;
		}

		public Task<List<Group>> GetCourseGroupsAsync(string courseId, bool includeArchived=false)
		{
			return GetCourseGroupsQueryable(courseId, includeArchived).ToListAsync();
		}

		public Task<List<Group>> GetMyGroupsFilterAccessibleToUserAsync(string courseId, ClaimsPrincipal user, bool includeArchived = false)
		{
			return GetMyGroupsFilterAccessibleToUserAsync(courseId, user.GetUserId(), includeArchived);
		}

		public Task<List<Group>> GetMyGroupsFilterAccessibleToUserAsync(string courseId, string userId, bool includeArchived = false)
		{
			var accessibleGroupsIds = db.GroupAccesses.Where(a => a.Group.CourseId == courseId && a.UserId == userId && a.IsEnabled).Select(a => a.GroupId);

			var groups = db.Groups.Where(g => g.CourseId == courseId && !g.IsDeleted && (g.OwnerId == userId || accessibleGroupsIds.Contains(g.Id)));
			if (!includeArchived)
				groups = groups.Where(g => !g.IsArchived);
			return groups.ToListAsync();
		}

		public Task<List<ApplicationUser>> GetGroupMembersAsUsersAsync(int groupId)
		{
			return db.GroupMembers.Include(m => m.User).Where(m => m.GroupId == groupId && !m.User.IsDeleted).Select(m => m.User).ToListAsync();
		}

		public Task<List<GroupMember>> GetGroupMembersAsync(int groupId)
		{
			return db.GroupMembers.Include(m => m.User).Where(m => m.GroupId == groupId && !m.User.IsDeleted).ToListAsync();
		}
		
		public Task<List<GroupMember>> GetGroupsMembersAsync(IEnumerable<int> groupsIds)
		{
			return db.GroupMembers.Include(m => m.User).Where(m => groupsIds.Contains(m.GroupId) && !m.User.IsDeleted).ToListAsync();
		}

		public async Task EnableInviteLinkAsync(int groupId, bool isEnabled)
		{
			var group = db.Groups.Find(groupId) ?? throw new ArgumentNullException($"Can't find group with id={groupId}");
			group.IsInviteLinkEnabled = isEnabled;
			
			await db.SaveChangesAsync().ConfigureAwait(false);
		}

		public Task<List<string>> GetUsersIdsForAllCourseGroupsAsync(string courseId)
		{
			var groupsIds = GetCourseGroupsQueryable(courseId).Select(g => g.Id);
			return db.GroupMembers.Where(m => groupsIds.Contains(m.GroupId)).Select(m => m.UserId).ToListAsync();
		}

		/* Return Dictionary<userId, List<groupId>> */
		public Task<Dictionary<string, List<int>>> GetUsersGroupsIdsAsync(string courseId, IEnumerable<string> usersIds)
		{
			var groupsIds = GetCourseGroupsQueryable(courseId).Select(g => g.Id);
			return db.GroupMembers
				.Where(m => groupsIds.Contains(m.GroupId) && usersIds.Contains(m.UserId))
				.GroupBy(m => m.UserId)
				.ToDictionaryAsync(g => g.Key, g => g.Select(m => m.GroupId).ToList());
		}

		public async Task<List<int>> GetUserGroupsIdsAsync(string courseId, string userId)
		{
			return (await GetUsersGroupsIdsAsync(courseId, new List<string> { userId }).ConfigureAwait(false))
				.GetOrDefault(userId, new List<int>());
		}

		public async Task<List<Group>> GetUserGroupsAsync(string courseId, string userId)
		{
			var userGroupsIds = await GetUserGroupsIdsAsync(courseId, userId).ConfigureAwait(false);
			return GetCourseGroupsQueryable(courseId).Where(g => userGroupsIds.Contains(g.Id)).ToList();
		}

		public async Task<bool> IsManualCheckingEnabledForUserAsync(Course course, string userId)
		{
			if (course.Settings.IsManualCheckingEnabled)
				return true;

			var userGroupsIds = await db.GroupMembers
				.Where(m => m.Group.CourseId == course.Id && m.UserId == userId && !m.Group.IsDeleted)
				.Select(m => m.GroupId)
				.Distinct()
				.ToListAsync()
				.ConfigureAwait(false);

			return await db.Groups.AnyAsync(g => userGroupsIds.Contains(g.Id) && g.IsManualCheckingEnabled).ConfigureAwait(false);
		}

		public async Task<bool> GetDefaultProhibitFurtherReviewForUserAsync(string courseId, string userId, ClaimsPrincipal instructor)
		{
			var accessibleGroups = await GetMyGroupsFilterAccessibleToUserAsync(courseId, instructor).ConfigureAwait(false);
			var userGroupsIds = await db.GroupMembers
				.Where(m => m.Group.CourseId == courseId && m.UserId == userId && !m.Group.IsDeleted)
				.Select(m => m.GroupId)
				.Distinct()
				.ToListAsync()
				.ConfigureAwait(false);

			var accessibleGroupsIds = accessibleGroups.Select(g => g.Id);			
			/* Return true if exists at least one group with enabled DefaultProhibitFurtherReview */
			return await db.Groups.AnyAsync(
				g => accessibleGroupsIds.Contains(g.Id) &&
					userGroupsIds.Contains(g.Id) &&
					g.DefaultProhibitFutherReview
			).ConfigureAwait(false);
		}

		public async Task EnableAdditionalScoringGroupsForGroupAsync(int groupId, IEnumerable<string> scoringGroupsIds)
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

				await db.SaveChangesAsync().ConfigureAwait(false);

				transaction.Commit();
			}
		}

		public Task<List<EnabledAdditionalScoringGroup>> GetEnabledAdditionalScoringGroupsAsync(string courseId)
		{
			var groupsIds = GetCourseGroupsQueryable(courseId).Select(g => g.Id);
			return db.EnabledAdditionalScoringGroups.Where(e => groupsIds.Contains(e.GroupId)).ToListAsync();
		}

		public Task<List<EnabledAdditionalScoringGroup>> GetEnabledAdditionalScoringGroupsForGroupAsync(int groupId)
		{
			return db.EnabledAdditionalScoringGroups.Where(e => e.GroupId == groupId).ToListAsync();
		}
	}
}