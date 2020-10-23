using System.Threading.Tasks;
using Database.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Database.Repos
{
	public class LtiConsumersRepo : ILtiConsumersRepo
	{
		private readonly UlearnDb db;
		private readonly ILogger logger;

		public LtiConsumersRepo(UlearnDb db, ILogger logger)
		{
			this.db = db;
			this.logger = logger;
		}

		[CanBeNull]
		public async Task<LtiConsumer> Find(string consumerKey)
		{
			return await db.Consumers.SingleOrDefaultAsync(consumer => consumer.Key == consumerKey);
		}
	}
}