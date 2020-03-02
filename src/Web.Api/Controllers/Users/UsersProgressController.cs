using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.CourseRoles;
using Database.Repos.Groups;
using Database.Repos.Users;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Ulearn.Common.Api.Models.Responses;
using Ulearn.Core.Courses;
using Ulearn.Web.Api.Models.Responses.User;

namespace Ulearn.Web.Api.Controllers.Users
{
	[Route("/users/progress")]
	public class UsersProgressController : BaseController
	{
		private readonly IVisitsRepo visitsRepo;
		private readonly IUserQuizzesRepo userQuizzesRepo;
		private readonly IAdditionalScoresRepo additionalScoresRepo;
		private readonly ICourseRolesRepo courseRolesRepo;
		private readonly IGroupAccessesRepo groupAccessesRepo;
		private readonly IGroupMembersRepo groupMembersRepo;

		public UsersProgressController(ILogger logger, IWebCourseManager courseManager, UlearnDb db, IUsersRepo usersRepo,
			IVisitsRepo visitsRepo, IUserQuizzesRepo userQuizzesRepo, IAdditionalScoresRepo additionalScoresRepo,
			ICourseRolesRepo courseRolesRepo, GroupAccessesRepo groupAccessesRepo, IGroupMembersRepo groupMembersRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			this.visitsRepo = visitsRepo;
			this.userQuizzesRepo = userQuizzesRepo;
			this.additionalScoresRepo = additionalScoresRepo;
			this.courseRolesRepo = courseRolesRepo;
			this.groupAccessesRepo = groupAccessesRepo;
			this.groupMembersRepo = groupMembersRepo;
		}
		
		/// <summary>
		/// Прогресс пользователей в курсе 
		/// </summary>
		[HttpGet("{courseId}")]
		public async Task<ActionResult<UsersProgressResponse>> UsersProgress(Course course, [FromBody]List<string> userIds)
		{
			if(!await IsUsersProgressVisibleForUser(course.Id, userIds))
				return NotFound(new ErrorResponse("Some users not found"));

			var shouldBeSolvedSlides = course.Slides.Where(s => s.ShouldBeSolved).Select(s => s.Id);
			var scores = await visitsRepo.GetScoresForSlides(course.Id, userIds, shouldBeSolvedSlides);
			var additionalScores = await GetAdditionalScores(course.Id, userIds).ConfigureAwait(false);

			var usersProgress = new Dictionary<string, UserProgress>();
			foreach (var userId in scores.Keys)
			{
				var slidesWithScore
					= scores[userId].ToDictionary(kvp => kvp.Key, kvp => new UserProgressSlideResult { Score = kvp.Value });
				var userAdditionalScores = additionalScores[userId];
				usersProgress[userId] = new UserProgress
				{
					SlidesWithScore = slidesWithScore,
					AdditionalScores = userAdditionalScores
				};
			}

			return new UsersProgressResponse
			{
				UsersProgress = usersProgress,
			};
		}

		private async Task<bool> IsUsersProgressVisibleForUser(string courseId, List<string> userIds)
		{
			var isSystemAdministrator = await IsSystemAdministratorAsync().ConfigureAwait(false);
			if (isSystemAdministrator)
				return true;
			if (await groupAccessesRepo.CanUserSeeAllCourseGroupsAsync(UserId, courseId))
				return true;
			var userRole = await courseRolesRepo.GetRoleAsync(UserId, courseId).ConfigureAwait(false);
			var groups = userRole == CourseRoleType.Instructor ? await groupAccessesRepo.GetAvailableForUserGroupsAsync(courseId, UserId, false, true, false) : new List<Group>();
			groups = groups.Concat(await groupMembersRepo.GetUserGroupsAsync(courseId, UserId, false)).Distinct().ToList();
			var members = (await groupMembersRepo.GetGroupsMembersIdsAsync(groups.Select(g => g.Id).ToList())).ToHashSet();
			return members.IsSupersetOf(userIds);
		}

		private async Task<Dictionary<string, Dictionary<Guid, Dictionary<string, int>>>> GetAdditionalScores(string courseId, List<string> userIds)
		{
			return (await additionalScoresRepo.GetAdditionalScoresForUsers(courseId, userIds).ConfigureAwait(false))
				.Select(kvp => (userId: kvp.Key.Item1, unitId: kvp.Key.Item2, scoringGroupId: kvp.Key.Item3, additionalScore: kvp.Value.Score))
				.GroupBy(t => t.userId)
				.ToDictSafe(g => g.Key,
					g =>
					{
						return g.GroupBy(t => t.unitId)
							.ToDictSafe(g2 => g2.Key,
								g2 =>
									g.ToDictSafe(t => t.scoringGroupId, t => t.additionalScore));
					});
		}
	}
}