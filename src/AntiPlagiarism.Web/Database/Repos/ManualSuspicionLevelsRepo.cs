using System;
using System.Threading.Tasks;
using System.Transactions;
using AntiPlagiarism.Web.Database.Models;
using Ulearn.Common;
using Ulearn.Common.Extensions;

namespace AntiPlagiarism.Web.Database.Repos
{
	public interface IManualSuspicionLevelsRepo
	{
		Task SetManualSuspicionLevelsAsync(ManualSuspicionLevels manualSuspicionLevels);
		Task<ManualSuspicionLevels> GetManualSuspicionLevelsAsync(Guid taskId, Language language);
	}

	public class ManualSuspicionLevelsRepo : IManualSuspicionLevelsRepo
	{
		private readonly AntiPlagiarismDb db;

		public ManualSuspicionLevelsRepo(AntiPlagiarismDb db)
		{
			this.db = db;
		}

		public async Task SetManualSuspicionLevelsAsync(ManualSuspicionLevels manualSuspicionLevels)
		{
			using (var ts = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(30), TransactionScopeAsyncFlowOption.Enabled))
			{
				db.AddOrUpdate(manualSuspicionLevels, p => p.TaskId == manualSuspicionLevels.TaskId);
				await db.SaveChangesAsync().ConfigureAwait(false);
				ts.Complete();
			}
		}

		public async Task<ManualSuspicionLevels> GetManualSuspicionLevelsAsync(Guid taskId, Language language)
		{
			return await db.ManualSuspicionLevels.FindAsync(taskId, language);
		}
	}
}