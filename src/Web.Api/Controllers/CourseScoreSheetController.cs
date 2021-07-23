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
using Microsoft.EntityFrameworkCore;
using Ulearn.Common;
using Ulearn.Common.Extensions;
using Ulearn.Core.Courses;
using Ulearn.Core.Courses.Slides;
using Ulearn.Web.Api.Models.Internal;
using Ulearn.Web.Api.Models.Parameters.Analytics;
using Ulearn.Web.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Ulearn.Core.Configuration;
using Ulearn.Core.Courses.Manager;
using Ulearn.Core.GoogleSheet;
using Ulearn.Web.Api.Models.Responses.Analytics;
using Web.Api.Configuration;


namespace Ulearn.Web.Api.Controllers
{
	public class CourseScoreSheetController : BaseController
	{
		private readonly ICourseRolesRepo courseRolesRepo;
		private readonly IGroupMembersRepo groupMembersRepo;
		private readonly IUnitsRepo unitsRepo;
		private readonly IGroupsRepo groupsRepo;
		private readonly ControllerUtils controllerUtils;
		private readonly IVisitsRepo visitsRepo;
		private readonly IAdditionalScoresRepo additionalScoresRepo;
		private readonly IGroupAccessesRepo groupAccessesRepo;
		private readonly UlearnConfiguration configuration;
		

		public CourseScoreSheetController(ICourseStorage courseStorage, UlearnDb db, IUsersRepo usersRepo, ICourseRolesRepo courseRolesRepo,
			IGroupMembersRepo groupMembersRepo, IUnitsRepo unitsRepo,
			IGroupsRepo groupsRepo, IGroupAccessesRepo groupAccessesRepo,
			ControllerUtils controllerUtils, IVisitsRepo visitsRepo, IAdditionalScoresRepo additionalScoresRepo,
			IOptions<WebApiConfiguration> options)
			: base(courseStorage, db, usersRepo)
		{
			this.courseRolesRepo = courseRolesRepo;
			this.groupMembersRepo = groupMembersRepo;
			this.unitsRepo = unitsRepo;
			this.groupsRepo = groupsRepo;
			this.groupAccessesRepo = groupAccessesRepo;
			this.controllerUtils = controllerUtils;
			this.visitsRepo = visitsRepo;
			this.additionalScoresRepo = additionalScoresRepo;
			configuration = options.Value;
		}
		
		[HttpGet("course-score-sheet/export/{fileNameWithNoExtension}.json")]
		[Authorize(Policy = "Instructors", AuthenticationSchemes = "Bearer,Identity.Application")]
		public async Task<ActionResult> ExportCourseStatisticsAsJson([FromQuery] CourseStatisticsParams param, [FromRoute] string fileNameWithNoExtension)
		{
			if (param.CourseId == null)
				return NotFound();

			var model = await StatisticModelUtils.GetCourseStatisticsModel(param, 3000, UserId, courseRolesRepo,courseStorage,unitsRepo,
				groupsRepo, controllerUtils, groupMembersRepo, groupAccessesRepo, visitsRepo, additionalScoresRepo, db);
			var serializedModel = new CourseStatisticsModel(model).JsonSerialize(Formatting.Indented);

			return File(Encoding.UTF8.GetBytes(serializedModel), "application/json", $"{fileNameWithNoExtension}.json");
		}
		
		[HttpGet("course-score-sheet/export/{fileNameWithNoExtension}.xml")]
		[Authorize(Policy = "Instructors", AuthenticationSchemes = "Bearer,Identity.Application")]
		public async Task<ActionResult> ExportCourseStatisticsAsXml([FromQuery] CourseStatisticsParams param, [FromRoute] string fileNameWithNoExtension)
		{
			if (param.CourseId == null)
				return NotFound();
			var model = await StatisticModelUtils.GetCourseStatisticsModel(param,3000, UserId, courseRolesRepo,courseStorage,unitsRepo,
				groupsRepo, controllerUtils, groupMembersRepo, groupAccessesRepo, visitsRepo, additionalScoresRepo, db);
			var serializedModel = new CourseStatisticsModel(model).XmlSerialize();
			return File(Encoding.UTF8.GetBytes(serializedModel), "text/xml", $"{fileNameWithNoExtension}.xml");
		}

		[HttpGet("course-score-sheet/export/{fileNameWithNoExtension}.xlsx")]
		[Authorize(Policy = "Instructors", AuthenticationSchemes = "Bearer,Identity.Application")]
		public async Task<ActionResult> ExportCourseStatisticsAsXlsx([FromQuery] CourseStatisticsParams param, [FromRoute] string fileNameWithNoExtension)
		{
			if (param.CourseId == null)
				return NotFound();

			var model = await StatisticModelUtils.GetCourseStatisticsModel(param, 3000, UserId, courseRolesRepo,courseStorage,unitsRepo,
				groupsRepo, controllerUtils, groupMembersRepo, groupAccessesRepo, visitsRepo, additionalScoresRepo, db);
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			var package = new ExcelPackage();
			var builder = new ExcelWorksheetBuilder(package.Workbook.Worksheets.Add(model.CourseTitle));
			StatisticModelUtils.FillCourseStatisticsWithBuilder(
				builder,
				model,
				exportEmails: true
			);
			builder = new ExcelWorksheetBuilder(package.Workbook.Worksheets.Add("Только полные баллы"));
			StatisticModelUtils.FillCourseStatisticsWithBuilder(
				builder,
				model,
				exportEmails: true,
				onlyFullScores: true
			);
			byte[] bytes;
			using (var stream = StaticRecyclableMemoryStreamManager.Manager.GetStream()) {
				await package.SaveAsAsync(stream);
				bytes = stream.ToArray();
			}
			
			return File(bytes, "application/vnd.ms-excel", $"{fileNameWithNoExtension}.xlsx");
		}

		[HttpPost("course-score-sheet/export/to-google-sheets")]
		[Authorize(Policy = "Instructors")]
		public async Task<ActionResult> ExportCourseStatisticsToGoogleSheets([FromBody] CourseStatisticsParams courseStatisticsParams)
		{
			if (courseStatisticsParams.CourseId == null)
				return NotFound();
			var model = await StatisticModelUtils.GetCourseStatisticsModel(courseStatisticsParams, 3000, UserId, courseRolesRepo,courseStorage,unitsRepo,
				groupsRepo, controllerUtils, groupMembersRepo, groupAccessesRepo, visitsRepo, additionalScoresRepo, db);
			var listId = courseStatisticsParams.ListId;
			var sheet = new GoogleSheet(200, 200, listId);
			var builder = new GoogleSheetBuilder(sheet);
			StatisticModelUtils.FillCourseStatisticsWithBuilder(builder, model);
			
			var credentialsJson = configuration.GoogleAccessCredentials;
			var client = new GoogleApiClient(credentialsJson);
			client.FillSpreadSheet(courseStatisticsParams.SpreadsheetId, sheet);
			return Ok();
		}
	}
}