using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AntiPlagiarism.Web.Database.Models;
using AntiPlagiarism.Web.Extensions;
using Microsoft.EntityFrameworkCore;
using Ulearn.Common;
using Ulearn.Common.Extensions;

namespace AntiPlagiarism.Web.Database.Repos
{
	public interface ITasksRepo
	{
		Task<List<Guid>> GetTaskIds();
		Task<TaskStatisticsParameters> FindTaskStatisticsParametersAsync(Guid taskId);
		Task SaveTaskStatisticsParametersAsync(TaskStatisticsParameters parameters);
	}
	
	public class TasksRepo : ITasksRepo
	{
		private readonly AntiPlagiarismDb db;

		public TasksRepo(AntiPlagiarismDb db)
		{
			this.db = db;
		}

		public Task<List<Guid>> GetTaskIds()
		{
			return db.TasksStatisticsParameters.Select(p => p.TaskId).ToListAsync();
		}

		public Task<TaskStatisticsParameters> FindTaskStatisticsParametersAsync(Guid taskId)
		{
			return db.TasksStatisticsParameters.FindAsync(taskId);
		}

		/* It's very important that SaveTaskStatisticsParametersAsync() works with disabled EF's Change Tracker */		
		public Task SaveTaskStatisticsParametersAsync(TaskStatisticsParameters parameters)
		{
			return FuncUtils.TrySeveralTimesAsync(
				async () =>
				{
					await TrySaveTaskStatisticsParametersAsync(parameters).ConfigureAwait(false);
					return true;
				},
				3,
				() => Task.Delay(TimeSpan.FromSeconds(1)),
				typeof(SqlException)
			);
		}

		private async Task TrySaveTaskStatisticsParametersAsync(TaskStatisticsParameters parameters)
		{
			using (var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable).ConfigureAwait(false))
			{
				db.AddOrUpdate(parameters, p => p.TaskId == parameters.TaskId);
				await db.SaveChangesAsync().ConfigureAwait(false);
				transaction.Commit();
			}
		}
	}
}