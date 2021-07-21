using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repos
{
	public class GoogleSheetExportTasksRepo : IGoogleSheetExportTasksRepo
	{
		private readonly UlearnDb db;

		public GoogleSheetExportTasksRepo(UlearnDb db)
		{
			this.db = db;
		}

		public async Task<GoogleSheetExportTask> AddTask(string courseId, string authorId,
			bool isVisibleForStudents, DateTime? refreshStartDate,
			DateTime? refreshEndDate, int? refreshTimeInMinutes,
			List<int> groupsIds, string spreadsheetId, int listId)
		{
			var exportTaskGroups = new List<GoogleSheetExportTaskGroup>();
			foreach (var groupId in groupsIds)
			{
				var exportTaskGroup = new GoogleSheetExportTaskGroup
				{
					GroupId = groupId
				};
				exportTaskGroups.Add(exportTaskGroup);
				db.GoogleSheetExportTaskGroups.Add(exportTaskGroup);
			}
			var exportTask = new GoogleSheetExportTask
			{
				CourseId = courseId,
				AuthorId = authorId,
				Groups = exportTaskGroups,
				IsVisibleForStudents = isVisibleForStudents,
				RefreshStartDate = refreshStartDate,
				RefreshEndDate = refreshEndDate,
				RefreshTimeInMinutes = refreshTimeInMinutes,
				SpreadsheetId = spreadsheetId,
				ListId = listId,
			};
			db.GoogleSheetExportTasks.Add(exportTask);
			await db.SaveChangesAsync();
			return exportTask;
		}

		public async Task<List<GoogleSheetExportTask>> GetTasks(string courseId, string authorId = null)
		{
			return await db.GoogleSheetExportTasks
				.Where(t => t.CourseId == courseId)
				.Where(t => authorId == null || t.AuthorId == authorId)
				.ToListAsync();
		}

		public async Task UpdateTask(int id, bool isVisibleForStudents, DateTime? refreshStartDate,
			DateTime? refreshEndDate, int? refreshTimeInMinutes)
		{
			var exportTask = await db.GoogleSheetExportTasks.FindAsync(id);

			if (exportTask == null)
				return;

			exportTask.IsVisibleForStudents = isVisibleForStudents;
			exportTask.RefreshStartDate = refreshStartDate;
			exportTask.RefreshEndDate = refreshEndDate;
			exportTask.RefreshTimeInMinutes = refreshTimeInMinutes;
			await db.SaveChangesAsync();
		}

		public async Task DeleteTask(int id)
		{
			var exportTask = await db.GoogleSheetExportTasks.FindAsync(id);
			
			if (exportTask == null)
				return;

			db.GoogleSheetExportTasks.Remove(exportTask); 
			await db.SaveChangesAsync();
		}
	}
}