using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Database.Repos;
using Database.Repos.Groups;
using Database.Repos.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ulearn.Common.Extensions;
using Ulearn.Core.Configuration;
using Ulearn.Core.Courses.Manager;
using Ulearn.Web.Api.Models;
using Ulearn.Web.Api.Models.Parameters;
using Ulearn.Web.Api.Models.Responses;
using Ulearn.Web.Api.Models.Responses.Courses;
using Ulearn.Web.Api.Utils;


namespace Ulearn.Web.Api.Controllers
{
	[Route("/course-score-sheet/export/to-google-sheets")]
	public class GoogleSheetsStatisticController : BaseController
	{
		private readonly ICourseRolesRepo courseRolesRepo;
		private readonly IGroupMembersRepo groupMembersRepo;
		private readonly IUnitsRepo unitsRepo;
		private readonly IGroupsRepo groupsRepo;
		private readonly ControllerUtils controllerUtils;
		private readonly IVisitsRepo visitsRepo;
		private readonly IAdditionalScoresRepo additionalScoresRepo;
		private readonly IGroupAccessesRepo groupAccessesRepo;
		private readonly IGoogleSheetExportTasksRepo googleSheetExportTasksRepo;

		public GoogleSheetsStatisticController(ICourseStorage courseStorage, UlearnDb db,
			IUsersRepo usersRepo, ICourseRolesRepo courseRolesRepo, IGroupMembersRepo groupMembersRepo,
			IUnitsRepo unitsRepo, IGroupsRepo groupsRepo, ControllerUtils controllerUtils, IVisitsRepo visitsRepo,
			IAdditionalScoresRepo additionalScoresRepo, IGroupAccessesRepo groupAccessesRepo, IOptions<UlearnConfiguration> configuration,
			IGoogleSheetExportTasksRepo googleSheetExportTasksRepo)
			: base(courseStorage, db, usersRepo)
		{
			this.courseRolesRepo = courseRolesRepo;
			this.groupMembersRepo = groupMembersRepo;
			this.unitsRepo = unitsRepo;
			this.groupsRepo = groupsRepo;
			this.controllerUtils = controllerUtils;
			this.visitsRepo = visitsRepo;
			this.additionalScoresRepo = additionalScoresRepo;
			this.groupAccessesRepo = groupAccessesRepo;
			this.googleSheetExportTasksRepo = googleSheetExportTasksRepo;
		}

		[HttpGet("tasks")]
		[Authorize(Policy = "Instructors")]
		public async Task<ActionResult<GoogleSheetsExportTaskListResponse>> GetAllCourseTasks([FromQuery] string courseId)
		{
			if (!await courseRolesRepo.HasUserAccessToCourse(UserId, courseId, CourseRoleType.Instructor))
				return Forbid();
			var isCourseAdmin = await courseRolesRepo.HasUserAccessToCourse(UserId, courseId, CourseRoleType.CourseAdmin);

			var exportTasks = await googleSheetExportTasksRepo.GetTasks(courseId, isCourseAdmin ? null : UserId);
			
			var responses = exportTasks
				.Select(exportTask => new GoogleSheetsExportTaskResponse
				{
					Id = exportTask.Id,
					AuthorInfo = BuildShortUserInfo(exportTask.Author),
					Groups = exportTask.Groups.Select(e => BuildShortGroupInfo(e.Group)).ToList(),
					IsVisibleForStudents = exportTask.IsVisibleForStudents,
					RefreshStartDate = exportTask.RefreshStartDate,
					RefreshEndDate = exportTask.RefreshEndDate,
					RefreshTimeInMinutes = exportTask.RefreshTimeInMinutes,
					SpreadsheetId = exportTask.SpreadsheetId,
					ListId = exportTask.ListId
				})
				.ToList();
			return new GoogleSheetsExportTaskListResponse
			{
				GoogleSheetsExportTasks = responses
			};
		}

		[HttpPost("tasks")]
		[Authorize]
		public async Task<ActionResult<GoogleSheetsExportTaskResponse>> AddNewTask([FromBody] GoogleSheetsExportTaskParams param)
		{
			if (!await HasAccessToGroups(param.CourseId, param.GroupsIds))
				return Forbid();

			var id = await googleSheetExportTasksRepo.AddTask(param.CourseId, UserId, param.IsVisibleForStudents,
				param.RefreshStartDate, param.RefreshEndDate, param.RefreshTimeInMinutes, param.GroupsIds,
				param.SpreadsheetId, param.ListId);

			var exportTask = await googleSheetExportTasksRepo.GetTaskById(id);

			var result = new GoogleSheetsExportTaskResponse
			{
				Id = exportTask.Id,
				AuthorInfo = BuildShortUserInfo(exportTask.Author),
				Groups = exportTask.Groups.Select(e => BuildShortGroupInfo(e.Group)).ToList(),
				IsVisibleForStudents = exportTask.IsVisibleForStudents,
				RefreshStartDate = exportTask.RefreshStartDate,
				RefreshEndDate = exportTask.RefreshEndDate,
				RefreshTimeInMinutes = exportTask.RefreshTimeInMinutes,
				SpreadsheetId = exportTask.SpreadsheetId,
				ListId = exportTask.ListId
			};
			return result;
		}

		// [HttpPost("tasks/{taskId}")]
		// [Authorize]
		// public async Task<ActionResult> ExportTaskNow([FromRoute] int taskId, [FromBody] string courseId)
		// {
		// 	var task = await googleSheetExportTasksRepo.GetTaskById(taskId);
		// 	if (task == null)
		// 		return NotFound();
		// 	if (!await courseRolesRepo.HasUserAccessToCourse(UserId, courseId, CourseRoleType.CourseAdmin) && task.AuthorId != UserId)
		// 		return Forbid();
		// 	
		// 	return Ok();
		// }
		
		[HttpPatch("tasks/{taskId}")]
		[Authorize]
		public async Task<ActionResult> UpdateTask([FromBody] GoogleSheetsExportTaskUpdateParams param, [FromRoute] int taskId)
		{
			var task = await googleSheetExportTasksRepo.GetTaskById(taskId);
			if (task == null)
				return NotFound();
			if (!await courseRolesRepo.HasUserAccessToCourse(UserId, param.CourseId, CourseRoleType.CourseAdmin) && task.AuthorId != UserId)
				return Forbid();
			await googleSheetExportTasksRepo.UpdateTask(task,
				param.IsVisibleForStudents, param.RefreshStartDate,
				param.RefreshEndDate, param.RefreshTimeInMinutes);
			return Ok();
		}

		[HttpDelete("tasks/{taskId}")]
		[Authorize(Policy = "Instructors")]
		public async Task<ActionResult> DeleteTask([FromBody] GoogleSheetsExportTaskUpdateParams param, [FromRoute] int taskId)
		{
			var task = await googleSheetExportTasksRepo.GetTaskById(taskId);
			if (task == null)
				return NotFound();
			if (!await courseRolesRepo.HasUserAccessToCourse(UserId, param.CourseId, CourseRoleType.CourseAdmin) && task.AuthorId != UserId)
				return Forbid();
			await googleSheetExportTasksRepo.DeleteTask(task);
			return NoContent();
		}

		private async Task<bool> HasAccessToGroups(string courseId, List<int> groupsIds)
		{
			if (!await courseRolesRepo.HasUserAccessToCourse(UserId, courseId, CourseRoleType.Instructor))
				return false;
			if (await courseRolesRepo.HasUserAccessToCourse(UserId, courseId, CourseRoleType.CourseAdmin))
				return true;
			var accessibleGroupsIds = (await groupAccessesRepo.GetAvailableForUserGroupsAsync(courseId, UserId, true, true,true))
				.Select(g => g.Id).ToHashSet();
			return groupsIds.TrueForAll(id => accessibleGroupsIds.Contains(id));
		}
	}
}