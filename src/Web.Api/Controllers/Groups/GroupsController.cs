using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Repos.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uLearn;
using Ulearn.Common.Extensions;
using Ulearn.Web.Api.Models.Parameters.Groups;
using Ulearn.Web.Api.Models.Responses;
using Ulearn.Web.Api.Models.Responses.Groups;
using ILogger = Serilog.ILogger;

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
		public async Task<IActionResult> GroupsList(Course course, [FromQuery] GroupsListParameters parameters)
		{
			return Json(await GetGroupsListResponseAsync(course, parameters).ConfigureAwait(false));
		}

		[HttpGet("in/{courseId}/archived")]
		[Authorize(Policy = "Instructors")]
		public async Task<IActionResult> ArchivedGroupsList(Course course, [FromQuery] GroupsListParameters parameters)
		{
			return Json(await GetGroupsListResponseAsync(course, parameters, onlyArchived: true).ConfigureAwait(false));
		}
		
		private async Task<GroupsListResponse> GetGroupsListResponseAsync(Course course, GroupsListParameters parameters, bool onlyArchived=false)
		{
			var groups = await groupAccessesRepo.GetAvailableForUserGroupsAsync(course.Id, User, onlyArchived).ConfigureAwait(false);
			/* Order groups by (name, createTime) and get one page of data (offset...offset+count) */
			var groupIds = groups.OrderBy(g => g.Name, StringComparer.InvariantCultureIgnoreCase).ThenBy(g => g.CreateTime)
				.Skip(parameters.Offset)
				.Take(parameters.Count)
				.Select(g => g.Id)
				.ToImmutableHashSet();
			var filteredGroups = groups.Where(g => groupIds.Contains(g.Id)).ToList();

			var groupMembers = await groupsRepo.GetGroupsMembersAsync(groupIds).ConfigureAwait(false);
			var membersCountByGroup = groupMembers.GroupBy(m => m.GroupId).ToDictionary(g => g.Key, g => g.Count()).ToDefaultDictionary();

			var groupAccessesByGroup = await groupAccessesRepo.GetGroupAccessesAsync(groupIds).ConfigureAwait(false);

			var groupInfos = filteredGroups.Select(g => BuildGroupInfo(
				g,
				membersCountByGroup[g.Id],
				groupAccessesByGroup[g.Id]
			)).ToList();
			
			var response = new GroupsListResponse
			{
				Groups = groupInfos,
				PaginationResponse = new PaginationResponse
				{
					Offset = parameters.Offset,
					Count = filteredGroups.Count,
					TotalCount = groups.Count,
				}
			};
			
			return response;
		}
	}
}