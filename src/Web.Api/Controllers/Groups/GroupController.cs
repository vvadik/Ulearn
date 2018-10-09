using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Repos.Groups;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Ulearn.Web.Api.Models.Responses.Groups;

namespace Ulearn.Web.Api.Controllers.Groups
{
	[Route("/groups/{groupId:int:min(0)}")]
	public class GroupController : BaseGroupController
	{
		private readonly GroupsRepo groupsRepo;
		private readonly GroupAccessesRepo groupAccessesRepo;

		public GroupController(ILogger logger, WebCourseManager courseManager, UlearnDb db, GroupsRepo groupsRepo, GroupAccessesRepo groupAccessesRepo)
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
		public async Task<IActionResult> Group(int groupId)
		{
			var group = await groupsRepo.FindGroupByIdAsync(groupId).ConfigureAwait(false);
			var members = await groupsRepo.GetGroupMembersAsync(groupId).ConfigureAwait(false);
			var accesses = await groupAccessesRepo.GetGroupAccessesAsync(groupId).ConfigureAwait(false);
			return Json(BuildGroupInfo(group, members.Count, accesses));
		}

		[HttpGet("students")]
		public async Task<IActionResult> GroupStudents(int groupId)
		{
			var members = await groupsRepo.GetGroupMembersAsync(groupId).ConfigureAwait(false);
			return Json(new GroupStudentsResponse
			{
				Students = members.Select(m => new GroupStudentInfo
				{
					User = BuildShortUserInfo(m.User, discloseLogin: true),
					AddingTime = m.AddingTime
				}).ToList()
			});
		}

		[HttpGet("accesses")]
		public async Task<IActionResult> GroupAccesses(int groupId)
		{
			var accesses = await groupAccessesRepo.GetGroupAccessesAsync(groupId).ConfigureAwait(false);
			return Json(new GroupAccessesResponse
			{
				Accesses = accesses.Select(BuildGroupAccessesInfo).ToList()
			});
		}
	}
}