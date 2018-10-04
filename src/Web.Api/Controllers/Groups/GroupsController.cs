using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Repos.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using uLearn;
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
			var groups = await groupAccessesRepo.GetAvailableForUserGroupsAsync(course.Id, User).ConfigureAwait(false);
			return Json(new GroupsListResponse
			{
				Groups = groups.Select(BuildGroupInfo).ToList()
			});
		}
		
		[HttpGet("in/{courseId}/archived")]
		[Authorize(Policy = "Instructors")]
		public async Task<IActionResult> ArchivedGroupsList(Course course)
		{
			var groups = await groupAccessesRepo.GetAvailableForUserGroupsAsync(course.Id, User, onlyArchived: true).ConfigureAwait(false);
			return Json(new GroupsListResponse
			{
				Groups = groups.Select(BuildGroupInfo).ToList()
			});
		}
	}
}