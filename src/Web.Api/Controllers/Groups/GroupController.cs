using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Database;
using Database.Extensions;
using Database.Models;
using Database.Repos;
using Database.Repos.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using Ulearn.Web.Api.Models.Parameters.Groups;
using Ulearn.Web.Api.Models.Responses;
using Ulearn.Web.Api.Models.Responses.Groups;

namespace Ulearn.Web.Api.Controllers.Groups
{
	[Route("/groups/{groupId:int:min(0)}")]
	[ProducesResponseType((int) HttpStatusCode.NotFound)]
	[ProducesResponseType((int) HttpStatusCode.Forbidden)]
	[Authorize]
	public class GroupController : BaseGroupController
	{
		private readonly IGroupsRepo groupsRepo;
		private readonly IGroupAccessesRepo groupAccessesRepo;
		private readonly IGroupMembersRepo groupMembersRepo;
		private readonly IUsersRepo usersRepo;
		private readonly IUserRolesRepo userRolesRepo;
		private readonly INotificationsRepo notificationsRepo;

		public GroupController(ILogger logger, WebCourseManager courseManager, UlearnDb db,
			IGroupsRepo groupsRepo, IGroupAccessesRepo groupAccessesRepo, IGroupMembersRepo groupMembersRepo, IUsersRepo usersRepo, IUserRolesRepo userRolesRepo, INotificationsRepo notificationsRepo)
			: base(logger, courseManager, db)
		{
			this.groupsRepo = groupsRepo;
			this.groupAccessesRepo = groupAccessesRepo;
			this.groupMembersRepo = groupMembersRepo;
			this.usersRepo = usersRepo;
			this.userRolesRepo = userRolesRepo;
			this.notificationsRepo = notificationsRepo;
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

		/// <summary>
		/// Информация о группе
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<GroupInfo>> Group(int groupId)
		{
			var group = await groupsRepo.FindGroupByIdAsync(groupId).ConfigureAwait(false);
			var members = await groupMembersRepo.GetGroupMembersAsync(groupId).ConfigureAwait(false);
			var accesses = await groupAccessesRepo.GetGroupAccessesAsync(groupId).ConfigureAwait(false);
			return BuildGroupInfo(group, members.Count, accesses);
		}

		/// <summary>
		/// Изменить группу
		/// </summary>
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

		/// <summary>
		/// Удалить группу
		/// </summary>
		[HttpDelete]
		public async Task<IActionResult> DeleteGroup(int groupId)
		{
			await groupsRepo.DeleteGroupAsync(groupId).ConfigureAwait(false);
			return Ok(new SuccessResponse("Group has been deleted"));
		}

		/// <summary>
		/// Сменить владельца группы
		/// </summary>
		[HttpPut("owner")]
		[SwaggerResponse((int) HttpStatusCode.NotFound, Description = "Can't find user or user is not an instructor")]
		public async Task<IActionResult> ChangeOwner(int groupId, [FromBody] ChangeOwnerParameters parameters)
		{
			var group = await groupsRepo.FindGroupByIdAsync(groupId).ConfigureAwait(false);
			
			var canChangeOwner = group.OwnerId == UserId || User.HasAccessFor(group.CourseId, CourseRole.CourseAdmin);
			if (!canChangeOwner)
				return StatusCode((int) HttpStatusCode.Forbidden, new ErrorResponse("You can't change the owner of this group. Only current owner and course admin can change the owner."));

			/* New owner should exist and be a course instructor */
			var user = await usersRepo.FindUserByIdAsync(parameters.OwnerId).ConfigureAwait(false);
			if (user == null)
				return NotFound(new ErrorResponse($"Can't find user with id {parameters.OwnerId}"));
			var isInstructor = await userRolesRepo.HasUserAccessToCourseAsync(parameters.OwnerId, group.CourseId, CourseRole.Instructor).ConfigureAwait(false);
			if (!isInstructor)
				return NotFound(new ErrorResponse($"User {parameters.OwnerId} is not an instructor of course {group.CourseId}"));
			
			/* Grant full access to previous owner */
			await groupAccessesRepo.GrantAccessAsync(groupId, group.OwnerId, GroupAccessType.FullAccess, UserId).ConfigureAwait(false);
			/* Change owner */
			await groupsRepo.ChangeGroupOwnerAsync(groupId, parameters.OwnerId).ConfigureAwait(false);
			/* Revoke access from new owner */
			await groupAccessesRepo.RevokeAccessAsync(groupId, parameters.OwnerId).ConfigureAwait(false);

			return Ok(new SuccessResponse($"New group's owner is {parameters.OwnerId}"));
		}

		/// <summary>
		/// Список студентов группы
		/// </summary>
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

		/// <summary>
		/// Добавить студента в группу
		/// </summary>
		[HttpPost("students/{studentId:guid}")]
		[SwaggerResponse((int) HttpStatusCode.NotFound, Description = "Can't find user")]
		[ProducesResponseType((int) HttpStatusCode.Conflict)]
		[SwaggerResponse((int) HttpStatusCode.Conflict, Description = "User is already a student of this group")]
		public async Task<IActionResult> AddStudent(int groupId, string studentId)
		{
			if (!User.IsSystemAdministrator())
				return StatusCode((int)HttpStatusCode.Forbidden, new ErrorResponse("Only system administrator can add students to group directly"));
			
			var user = await usersRepo.FindUserByIdAsync(studentId).ConfigureAwait(false);
			if (user == null)
				return NotFound(new ErrorResponse($"Can't find user with id {studentId}"));
			
			var groupMember = await groupMembersRepo.AddUserToGroupAsync(groupId, studentId).ConfigureAwait(false);
			if (groupMember == null)
				return StatusCode((int)HttpStatusCode.Conflict, new ErrorResponse($"User {studentId} is already a student of group {groupId}"));

			return Ok(new SuccessResponse($"Student {studentId} is added to group {groupId}"));
		}
		
		/// <summary>
		/// Удалить студента из группы 
		/// </summary>
		[HttpDelete("students/{studentId:guid}")]
		[SwaggerResponse((int) HttpStatusCode.NotFound, Description = "Can't find user or user is not a student of this group")]
		public async Task<IActionResult> RemoveStudent(int groupId, string studentId)
		{
			var group = await groupsRepo.FindGroupByIdAsync(groupId).ConfigureAwait(false);
			
			var user = await usersRepo.FindUserByIdAsync(studentId).ConfigureAwait(false);
			if (user == null)
				return NotFound(new ErrorResponse($"Can't find user with id {studentId}"));

			var groupMember = await groupMembersRepo.RemoveUserFromGroupAsync(groupId, studentId).ConfigureAwait(false);
			if (groupMember == null)
				return NotFound(new ErrorResponse($"User {studentId} is not a student of group {groupId}"));
			
			await notificationsRepo.AddNotificationAsync(
				group.CourseId,
				new GroupMembersHaveBeenRemovedNotification(groupId, new List<string> { studentId }, usersRepo),
				UserId
			).ConfigureAwait(false);

			return Ok(new SuccessResponse($"Student {studentId} is removed from group {groupId}"));
		}

		/// <summary>
		/// Список доступов к группе
		/// </summary>
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