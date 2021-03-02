using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using AntiPlagiarism.Web.Database.Models;
using Microsoft.EntityFrameworkCore;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace AntiPlagiarism.Web.Database.Repos
{
	public interface IWorkQueueRepo
	{
		Task Add(QueueIds queueId, string itemId);
		Task<WorkQueueItem> TakeNoTracking(QueueIds queueId, TimeSpan? timeLimit = null);
		Task Remove(int id);
	}
	
	public class WorkQueueRepo : IWorkQueueRepo
	{
		private readonly AntiPlagiarismDb db;

		public WorkQueueRepo(AntiPlagiarismDb db)
		{
			this.db = db;
		}

		public async Task Add(QueueIds queueId, string itemId)
		{
			db.WorkQueueItems.Add(new WorkQueueItem {QueueId = queueId, ItemId = itemId});
			await db.SaveChangesAsync().ConfigureAwait(false);
		}

		public async Task Remove(int id)
		{
			var itemToRemove = db.WorkQueueItems.SingleOrDefault(x => x.Id == id);
			if (itemToRemove != null) {
				db.WorkQueueItems.Remove(itemToRemove);
				await db.SaveChangesAsync().ConfigureAwait(false);
			}
		}

		// http://rusanu.com/2010/03/26/using-tables-as-queues/
		public async Task<WorkQueueItem> TakeNoTracking(QueueIds queueId, TimeSpan? timeLimit = null)
		{
			timeLimit = timeLimit ?? TimeSpan.FromMinutes(5);
			// readpast пропускает заблокированные строки
			// rowlock подсказывает блокировать строки, а не страницы
			var sql = 
$@"with cte as (
	select top(1) *
	from {AntiPlagiarismDb.DefaultSchema}.{nameof(db.WorkQueueItems)} with (rowlock, readpast)
	where {WorkQueueItem.QueueIdColumnName} = @queueId
	and ({WorkQueueItem.TakeAfterTimeColumnName} is NULL or {WorkQueueItem.TakeAfterTimeColumnName} < @now)
	order by {WorkQueueItem.IdColumnName}
)
update cte SET {WorkQueueItem.TakeAfterTimeColumnName} = @timeLimit
output inserted.*";
			using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead }, TransactionScopeAsyncFlowOption.Enabled))
			{
				var taken = (await db.WorkQueueItems.FromSqlRaw(
					sql,
					new SqlParameter("@queueId", queueId),
					new SqlParameter("@now", DateTime.UtcNow),
					new SqlParameter("@timeLimit", DateTime.UtcNow + timeLimit)
				).AsNoTracking().ToListAsync()).FirstOrDefault();
				scope.Complete();
				return taken;
			}
		}
	}
}