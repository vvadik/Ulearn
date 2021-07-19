using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ulearn.Web.Api.Models;
using Ulearn.Web.Api.Models.Parameters;
using Ulearn.Web.Api.Models.Responses;
using Ulearn.Web.Api.Models.Responses.Courses;


namespace Ulearn.Web.Api.Controllers
{
	[Route("/course-score-sheet/export/to-google-sheets")]
	public class GoogleSheetsStatisticController
	{
		[HttpGet("/tasks")]
		[Authorize(Policy = "Instructors")]
		public async Task<ActionResult<GoogleSheetsTaskListResponse>> GetAllTasks([FromQuery] GoogleSheetsTaskParams param)
		{
			throw new NotImplementedException();
		}
		
		[HttpGet("/tasks/{taskId}")]
		[Authorize(Policy = "Instructors")]
		public async Task<ActionResult<GoogleSheetsTaskResponse>> GetOneTask([FromQuery] GoogleSheetsTaskParams param)
		{
			throw new NotImplementedException();
		}
		
		[HttpPost("/tasks")]
		[Authorize(Policy = "Instructors")]
		public async Task<ActionResult<GoogleSheetsTaskResponse>> AddNewTask([FromQuery] GoogleSheetsTaskParams param)
		{
			throw new NotImplementedException();
		}
		
		[HttpPatch("/tasks/{taskId}")]
		[Authorize(Policy = "Instructors")]
		public async Task<ActionResult<GoogleSheetsTaskResponse>> PatchTask([FromQuery] GoogleSheetsTaskParams param)
		{
			throw new NotImplementedException();
		}
		
		[HttpDelete("/tasks/{taskId}")]
		[Authorize(Policy = "Instructors")]
		public async Task<ActionResult> DeleteTask([FromQuery] GoogleSheetsTaskParams param)
		{
			throw new NotImplementedException();
		}
	}
}