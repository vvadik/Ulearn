using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Database.Repos
{
	public class ConsumersRepo : IConsumersRepo
	{
		private readonly UlearnDb db;

		public ConsumersRepo(UlearnDb db)
		{
			this.db = db;
		}

		[CanBeNull]
		public Task<LtiConsumer> FindAsync(string consumerKey)
		{
			return db.Consumers.SingleOrDefaultAsync(consumer => consumer.Key == consumerKey);
		}
	}
}