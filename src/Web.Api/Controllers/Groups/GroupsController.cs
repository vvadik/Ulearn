using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Repos.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using uLearn;
using Ulearn.Common.Extensions;
using Ulearn.Web.Api.Models.Responses.Groups;

namespace Ulearn.Web.Api.Controllers.Groups
{
	[Route("/groups")]
	public class GroupsController : BaseGroupController
	{
		private readonly GroupsRepo groupsRepo;
		private readonly GroupAccessesRepo groupAccessesRepo;

		public GroupsController(ILogger logger, WebCourseManager courseManager, UlearnDb db,
			GroupsRepo groupsRepo, GroupAccessesRepo groupAccessesRepo)
			: base(logger, courseManager, db)
		{
			this.groupsRepo = groupsRepo;
			this.groupAccessesRepo = groupAccessesRepo;
		}

		[HttpGet("in/{courseId}")]
		[Authorize(Policy = "Instructors")]
		public async Task<IActionResult> GroupsList(Course course)
		{
			return Json(await GetGroupsListResponseAsync(course).ConfigureAwait(false));
		}

		[HttpGet("in/{courseId}/archived")]
		[Authorize(Policy = "Instructors")]
		public async Task<IActionResult> ArchivedGroupsList(Course course)
		{
			return Json(await GetGroupsListResponseAsync(course, onlyArchived: true).ConfigureAwait(false));
		}
		
		private async Task<GroupsListResponse> GetGroupsListResponseAsync(Course course, bool onlyArchived=false)
		{
			var groups = await groupAccessesRepo.GetAvailableForUserGroupsAsync(course.Id, User, onlyArchived).ConfigureAwait(false);
			var groupIds = groups.Select(g => g.Id).ToList();

			var groupMembers = await groupsRepo.GetGroupsMembersAsync(groupIds).ConfigureAwait(false);
			var membersCountByGroup = groupMembers.GroupBy(m => m.GroupId).ToDictionary(g => g.Key, g => g.Count()).ToDefaultDictionary();

			var groupAccessesByGroup = await groupAccessesRepo.GetGroupAccessesAsync(groupIds).ConfigureAwait(false);

			return new GroupsListResponse
			{
				Groups = groups.Select(g => BuildGroupInfo(
					g,
					membersCountByGroup[g.Id],
					groupAccessesByGroup[g.Id]
				)).ToList()
			};
		}
	}
}