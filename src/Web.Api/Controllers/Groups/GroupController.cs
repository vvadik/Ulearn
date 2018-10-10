using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Database;
using Database.Repos.Groups;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Ulearn.Web.Api.Models.Parameters.Groups;
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

		public GroupController(ILogger logger, WebCourseManager courseManager, UlearnDb db, IGroupsRepo groupsRepo, IGroupAccessesRepo groupAccessesRepo)
			: base(logger, courseManager, db)
		{
			this.groupsRepo = groupsRepo;
			this.groupAccessesRepo = groupAccessesRepo;
		}

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var groupId = (int) context.ActionArguments["groupId"];
			
			var group = await groupsRepo.FindGroupByIdAsync(groupId).ConfigureAwait(false);
			if (group == null)
			{
				context.Result = NotFound();
				return;
			}

			var isAvailable = await groupAccessesRepo.IsGroupAvailableForUserAsync(group, User).ConfigureAwait(false);
			if (!isAvailable)
			{
				context.Result = Forbid();
				return;
			}

			context.ActionArguments["group"] = group;

			await base.OnActionExecutionAsync(context, next).ConfigureAwait(false);
		}

		[HttpGet]
		public async Task<ActionResult<GroupInfo>> Group(int groupId)
		{
			var group = await groupsRepo.FindGroupByIdAsync(groupId).ConfigureAwait(false);
			var members = await groupsRepo.GetGroupMembersAsync(groupId).ConfigureAwait(false);
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

			return BuildGroupInfo(await groupsRepo.FindGroupByIdAsync(groupId).ConfigureAwait(false));
		}

		[HttpDelete]
		public async Task<IActionResult> DeleteGroup(int groupId)
		{
			await groupsRepo.DeleteGroupAsync(groupId).ConfigureAwait(false);
			return Ok();
		}

		[HttpGet("students")]
		public async Task<ActionResult<GroupStudentsResponse>> GroupStudents(int groupId)
		{
			var members = await groupsRepo.GetGroupMembersAsync(groupId).ConfigureAwait(false);
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