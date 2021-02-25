using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using AntiPlagiarism.Web.Database.Extensions;
using AntiPlagiarism.Web.Database.Models;
using Microsoft.EntityFrameworkCore;
using Ulearn.Common;
using Z.EntityFramework.Plus;

namespace AntiPlagiarism.Web.Database.Repos
{
	public interface ITasksRepo
	{
		Task<List<Guid>> GetTaskIds();
		Task<TaskStatisticsParameters> FindTaskStatisticsParametersAsync(Guid taskId);
		Task SaveTaskStatisticsParametersAsync(TaskStatisticsParameters parameters, List<TaskStatisticsSourceData> sourceData);
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

		public async Task<TaskStatisticsParameters> FindTaskStatisticsParametersAsync(Guid taskId)
		{
			return await db.TasksStatisticsParameters.FindAsync(taskId);
		}

		/* It's very important that SaveTaskStatisticsParametersAsync() works with disabled EF's Change Tracker */
		public Task SaveTaskStatisticsParametersAsync(TaskStatisticsParameters parameters, List<TaskStatisticsSourceData> sourceData)
		{
			return FuncUtils.TrySeveralTimesAsync(
				async () =>
				{
					await TrySaveTaskStatisticsParametersAsync(parameters, sourceData).ConfigureAwait(false);
					return true;
				},
				3,
				() => Task.Delay(TimeSpan.FromSeconds(1)),
				typeof(SqlException)
			);
		}

		private async Task TrySaveTaskStatisticsParametersAsync(TaskStatisticsParameters parameters, List<TaskStatisticsSourceData> sourceData)
		{
			using (var ts = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(30), TransactionScopeAsyncFlowOption.Enabled))
			{
				await db.TaskStatisticsSourceData
					.Where(d => d.Submission1.TaskId == parameters.TaskId)
					.DeleteAsync();
				db.AddOrUpdate(parameters, p => p.TaskId == parameters.TaskId);
				db.TaskStatisticsSourceData.AddRange(sourceData);
				await db.SaveChangesAsync().ConfigureAwait(false);
				ts.Complete();
			}
		}
	}
}