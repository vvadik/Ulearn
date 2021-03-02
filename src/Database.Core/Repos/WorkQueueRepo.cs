using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Database.Models;
using JetBrains.Annotations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Database.Repos
{
	public class WorkQueueRepo : IWorkQueueRepo
	{
		private readonly UlearnDb db;

		public WorkQueueRepo(UlearnDb db)
		{
			this.db = db;
		}

		public async Task Add(int queueId, string itemId, string type, int priority = 0)
		{
			db.WorkQueueItems.Add(new WorkQueueItem {QueueId = queueId, ItemId = itemId, Type = type, Priority = priority});
			await db.SaveChangesAsync();
		}

		public async Task Remove(int id)
		{
			await db.WorkQueueItems.Where(i => i.Id == id).DeleteAsync();
		}

		public async Task RemoveByItemId(int queueId, string itemId)
		{
			await db.WorkQueueItems.Where(i => i.QueueId == queueId && i.ItemId == itemId).DeleteAsync();
		}

		public async Task ReturnToQueue(int id)
		{
			await db.WorkQueueItems.Where(i => i.Id == id).UpdateAsync(c => new WorkQueueItem {TakeAfterTime = null});
		}

		// http://rusanu.com/2010/03/26/using-tables-as-queues/
		[ItemCanBeNull]
		public async Task<WorkQueueItem> TakeNoTracking(int queueId, List<string> types, TimeSpan? timeLimit = null)
		{
			timeLimit = timeLimit ?? TimeSpan.FromMinutes(5);
			// readpast пропускает заблокированные строки
			// rowlock подсказывает блокировать строки, а не страницы
			var typesCondition = types.Count == 0
				? ""
				: $"and {WorkQueueItem.TypeColumnName} IN ({string.Join(", ", types.Select(t => $"'{t}'"))})";
			var sql = 
$@"with cte as (
	select top(1) *
	from dbo.{nameof(db.WorkQueueItems)} with (rowlock, readpast)
	where {WorkQueueItem.QueueIdColumnName} = @queueId
	and ({WorkQueueItem.TakeAfterTimeColumnName} is NULL or {WorkQueueItem.TakeAfterTimeColumnName} < @now)
	{typesCondition}
	order by {WorkQueueItem.PriorityColumnName} desc, {WorkQueueItem.IdColumnName}
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