using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Database;
using Database.Repos;
using Database.Repos.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses;
using Ulearn.Web.Api.Models.Parameters.Groups;
using Ulearn.Web.Api.Models.Responses;
using Ulearn.Web.Api.Models.Responses.Groups;
using ILogger = Serilog.ILogger;

namespace Ulearn.Web.Api.Controllers.Groups
{
	[Route("/groups")]
	public class GroupsController : BaseGroupController
	{
		private readonly IGroupsRepo groupsRepo;
		private readonly IGroupAccessesRepo groupAccessesRepo;
		private readonly IGroupMembersRepo groupMembersRepo;

		public GroupsController(ILogger logger, IWebCourseManager courseManager, UlearnDb db,
			IUsersRepo usersRepo,
			IGroupsRepo groupsRepo, IGroupAccessesRepo groupAccessesRepo, IGroupMembersRepo groupMembersRepo)
			: base(logger, courseManager, db, usersRepo)
		{
			this.groupsRepo = groupsRepo;
			this.groupAccessesRepo = groupAccessesRepo;
			this.groupMembersRepo = groupMembersRepo;
		}

		/// <summary>
		/// Список неархивных групп в курсе
		/// </summary>
		[HttpGet("in/{courseId}")]
		[Authorize(Policy = "Instructors")]
		public async Task<ActionResult<GroupsListResponse>> GroupsList(ICourse course, [FromQuery] GroupsListParameters parameters)
		{
			return await GetGroupsListResponseAsync(course, parameters).ConfigureAwait(false);
		}

		/// <summary>
		/// Список архивных группу в курсе
		/// </summary>
		[HttpGet("in/{courseId}/archived")]
		[Authorize(Policy = "Instructors")]
		public async Task<ActionResult<GroupsListResponse>> ArchivedGroupsList(ICourse course, [FromQuery] GroupsListParameters parameters)
		{
			return await GetGroupsListResponseAsync(course, parameters, onlyArchived: true).ConfigureAwait(false);
		}
		
		private async Task<GroupsListResponse> GetGroupsListResponseAsync(ICourse course, GroupsListParameters parameters, bool onlyArchived=false)
		{
			var groups = await groupAccessesRepo.GetAvailableForUserGroupsAsync(course.Id, UserId, onlyArchived).ConfigureAwait(false);
			/* Order groups by (name, createTime) and get one page of data (offset...offset+count) */
			var groupIds = groups.OrderBy(g => g.Name, StringComparer.InvariantCultureIgnoreCase).ThenBy(g => g.CreateTime)
				.Skip(parameters.Offset)
				.Take(parameters.Count)
				.Select(g => g.Id)
				.ToImmutableHashSet();
			var filteredGroups = groups.Where(g => groupIds.Contains(g.Id)).ToList();

			var groupMembers = await groupMembersRepo.GetGroupsMembersAsync(groupIds).ConfigureAwait(false);
			var membersCountByGroup = groupMembers.GroupBy(m => m.GroupId).ToDictionary(g => g.Key, g => g.Count()).ToDefaultDictionary();

			var groupAccessesByGroup = await groupAccessesRepo.GetGroupAccessesAsync(groupIds).ConfigureAwait(false);

			var groupInfos = filteredGroups.Select(g => BuildGroupInfo(
				g,
				membersCountByGroup[g.Id],
				groupAccessesByGroup[g.Id],
				addGroupApiUrl: true
			)).ToList();
			
			return new GroupsListResponse
			{
				Groups = groupInfos,
				PaginationResponse = new PaginationResponse
				{
					Offset = parameters.Offset,
					Count = filteredGroups.Count,
					TotalCount = groups.Count,
				}
			};
		}

		/// <summary>
		/// Создать новую группу в курсе
		/// </summary>
		/// <param name="parameters">Название новой группы</param>
		[HttpPost("in/{courseId}")]
		[Authorize(Policy = "Instructors")]
		[ProducesResponseType((int) HttpStatusCode.Created)]
		public async Task<ActionResult<CreateGroupResponse>> CreateGroup([FromRoute] Course course, CreateGroupParameters parameters)
		{
			var ownerId = User.GetUserId();
			var group = await groupsRepo.CreateGroupAsync(course.Id, parameters.Name, ownerId).ConfigureAwait(false);

			var url = Url.Action(new UrlActionContext { Action = nameof(GroupController.Group), Controller = "Group", Values = new { groupId = group.Id }});
			return Created(url, new CreateGroupResponse
			{
				GroupId = group.Id,
				ApiUrl = url,
			});
		}
	}
}