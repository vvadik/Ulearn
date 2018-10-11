using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Database;
using Database.Extensions;
using Database.Models;
using Database.Repos;
using Database.Repos.Groups;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Ulearn.Common.Extensions;
using Ulearn.Web.Api.Models.Parameters.Groups;
using Ulearn.Web.Api.Models.Responses;
using Ulearn.Web.Api.Models.Responses.Groups;

namespace Ulearn.Web.Api.Controllers.Groups
{
	[Route("/groups/{groupId:int:min(0)}")]
	[ProducesResponseType((int) HttpStatusCode.NotFound)]
	[ProducesResponseType((int) HttpStatusCode.Forbidden)]
	public class GroupController : BaseGroupController
	{
		private readonly IGroupsRepo groupsRepo;
		private readonly IGroupAccessesRepo groupAccessesRepo;
		private readonly IGroupMembersRepo groupMembersRepo;
		private readonly IUsersRepo usersRepo;
		private readonly IUserRolesRepo userRolesRepo;

		public GroupController(ILogger logger, WebCourseManager courseManager, UlearnDb db,
			IGroupsRepo groupsRepo, IGroupAccessesRepo groupAccessesRepo, IGroupMembersRepo groupMembersRepo, IUsersRepo usersRepo, IUserRolesRepo userRolesRepo)
			: base(logger, courseManager, db)
		{
			this.groupsRepo = groupsRepo;
			this.groupAccessesRepo = groupAccessesRepo;
			this.groupMembersRepo = groupMembersRepo;
			this.usersRepo = usersRepo;
			this.userRolesRepo = userRolesRepo;
		}

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var groupId = (int) context.ActionArguments["groupId"];
			
			var group = await groupsRepo.FindGroupByIdAsync(groupId).ConfigureAwait(false);
			if (group == null)
			{
				context.Result = NotFound(new ErrorResponse($"Group with id {groupId} not found"));
				return;
			}

			var isAvailable = await groupAccessesRepo.IsGroupAvailableForUserAsync(group, User).ConfigureAwait(false);
			if (!isAvailable)
			{
				context.Result = StatusCode((int)HttpStatusCode.Forbidden, new ErrorResponse("You have no access to this group"));
				return;
			}

			context.ActionArguments["group"] = group;

			await base.OnActionExecutionAsync(context, next).ConfigureAwait(false);
		}

		[HttpGet]
		public async Task<ActionResult<GroupInfo>> Group(int groupId)
		{
			var group = await groupsRepo.FindGroupByIdAsync(groupId).ConfigureAwait(false);
			var members = await groupMembersRepo.GetGroupMembersAsync(groupId).ConfigureAwait(false);
			var accesses = await groupAccessesRepo.GetGroupAccessesAsync(groupId).ConfigureAwait(false);
			return BuildGroupInfo(group, members.Count, accesses);
		}

		[HttpPatch]
		public async Task<ActionResult<GroupInfo>> UpdateGroup(int groupId, [FromBody] UpdateGroupParameters parameters)
		{
			var group = await groupsRepo.FindGroupByIdAsync(groupId).ConfigureAwait(false);
			
			var newName = parameters.Name ?? group.Name;
			var newIsManualCheckingEnabled = parameters.IsManualCheckingEnabled ?? group.IsManualCheckingEnabled;
			var newIsManualCheckingEnabledForOldSolutions = parameters.IsManualCheckingEnabledForOldSolutions ?? group.IsManualCheckingEnabledForOldSolutions;
			var newDefaultProhibitFurtherReview = parameters.DefaultProhibitFurtherReview ?? group.DefaultProhibitFutherReview;
			var newCanUsersSeeGroupProgress = parameters.CanStudentsSeeGroupProgress ?? group.CanUsersSeeGroupProgress;
			await groupsRepo.ModifyGroupAsync(
				groupId,
				newName,
				newIsManualCheckingEnabled,
				newIsManualCheckingEnabledForOldSolutions,
				newDefaultProhibitFurtherReview,
				newCanUsersSeeGroupProgress
			).ConfigureAwait(false);

			if (parameters.IsArchived.HasValue)
				await groupsRepo.ArchiveGroupAsync(groupId, parameters.IsArchived.Value).ConfigureAwait(false);

			if (parameters.IsInviteLinkEnabled.HasValue)
				await groupsRepo.EnableInviteLinkAsync(groupId, parameters.IsInviteLinkEnabled.Value).ConfigureAwait(false);

			return BuildGroupInfo(await groupsRepo.FindGroupByIdAsync(groupId).ConfigureAwait(false));
		}

		[HttpDelete]
		public async Task<IActionResult> DeleteGroup(int groupId)
		{
			await groupsRepo.DeleteGroupAsync(groupId).ConfigureAwait(false);
			return Ok(new SuccessResponse("Group has been deleted"));
		}

		[HttpPut("owner")]
		public async Task<IActionResult> ChangeOwner(int groupId, [FromBody] ChangeOwnerParameters parameters)
		{
			var group = await groupsRepo.FindGroupByIdAsync(groupId).ConfigureAwait(false);
			
			var canChangeOwner = group.OwnerId == UserId || User.HasAccessFor(group.CourseId, CourseRole.CourseAdmin);
			if (!canChangeOwner)
				return StatusCode((int) HttpStatusCode.Forbidden, new ErrorResponse("You can't change the owner of this group. Only current owner and course admin can change the owner."));

			/* New owner should exist and be a course instructor */
			var user = usersRepo.FindUserById(parameters.OwnerId);
			if (user == null)
				return BadRequest(new ErrorResponse($"Can't find user with id {parameters.OwnerId}"));
			var isInstructor = await userRolesRepo.HasUserAccessToCourseAsync(parameters.OwnerId, group.CourseId, CourseRole.Instructor).ConfigureAwait(false);
			if (!isInstructor)
				return BadRequest(new ErrorResponse($"User {parameters.OwnerId} is not an instructor of course {group.CourseId}"));
			
			/* Grant full access to previous owner */
			await groupAccessesRepo.GrantAccessAsync(groupId, group.OwnerId, GroupAccessType.FullAccess, UserId).ConfigureAwait(false);
			/* Change owner */
			await groupsRepo.ChangeGroupOwnerAsync(groupId, parameters.OwnerId).ConfigureAwait(false);
			/* Revoke access from new owner */
			await groupAccessesRepo.RevokeAccessAsync(groupId, parameters.OwnerId).ConfigureAwait(false);

			return Ok(new SuccessResponse($"New group's owner is {parameters.OwnerId}"));
		}

		[HttpGet("students")]
		public async Task<ActionResult<GroupStudentsResponse>> GroupStudents(int groupId)
		{
			var members = await groupMembersRepo.GetGroupMembersAsync(groupId).ConfigureAwait(false);
			return new GroupStudentsResponse
			{
				Students = members.Select(m => new GroupStudentInfo
				{
					User = BuildShortUserInfo(m.User, discloseLogin: true),
					AddingTime = m.AddingTime
				}).ToList()
			};
		}

		[HttpGet("accesses")]
		public async Task<ActionResult<GroupAccessesResponse>> GroupAccesses(int groupId)
		{
			var accesses = await groupAccessesRepo.GetGroupAccessesAsync(groupId).ConfigureAwait(false);
			return new GroupAccessesResponse
			{
				Accesses = accesses.Select(BuildGroupAccessesInfo).ToList()
			};
		}
	}
}