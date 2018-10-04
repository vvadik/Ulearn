using System;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using uLearn;
using Ulearn.Web.Api.Models.Responses.Groups;

namespace Ulearn.Web.Api.Controllers.Groups
{
	[Route("/groups")]
	public class GroupsController : BaseController
	{
		private readonly GroupsRepo groupsRepo;

		public GroupsController(ILogger logger, WebCourseManager courseManager, UlearnDb db, GroupsRepo groupsRepo)
			: base(logger, courseManager, db)
		{
			this.groupsRepo = groupsRepo;
		}

		[HttpGet("in/{courseId}")]
		[Authorize(Policy = "Instructors")]
		public async Task<IActionResult> GroupsList(Course course)
		{
			var groups = await groupsRepo.GetAvailableForUserGroupsAsync(course.Id, User).ConfigureAwait(false);
			return Json(new GroupsListResponse
			{
				Groups = groups.Select(BuildGroupInfo).ToList()
			});
		}
		
		[HttpGet("in/{courseId}/archived")]
		[Authorize(Policy = "Instructors")]
		public async Task<IActionResult> ArchivedGroupsList(Course course)
		{
			var groups = await groupsRepo.GetAvailableForUserGroupsAsync(course.Id, User, onlyArchived: true).ConfigureAwait(false);
			return Json(new GroupsListResponse
			{
				Groups = groups.Select(BuildGroupInfo).ToList()
			});
		}

		private GroupInfo BuildGroupInfo(Group group)
		{
			if (group == null)
				throw new ArgumentNullException(nameof(group));

			return new GroupInfo
			{
				Id = group.Id,
				CreateTime = group.CreateTime,
				Name = group.Name,
				Owner = BuildShortUserInfo(group.Owner),
				InviteHash = group.InviteHash,
				IsInviteLinkEnabled = group.IsInviteLinkEnabled,
				IsArchived = group.IsArchived,
				
				IsManualCheckingEnabled = group.IsManualCheckingEnabled,
				IsManualCheckingEnabledForOldSolutions = group.IsManualCheckingEnabledForOldSolutions,
				DefaultProhibitFurtherReview = group.DefaultProhibitFutherReview,
				CanUsersSeeGroupProgress = group.CanUsersSeeGroupProgress,
			};
		}
	}
}