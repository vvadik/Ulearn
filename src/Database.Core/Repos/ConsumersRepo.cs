using System.Linq;
using Database.Models;

namespace Database.Repos
{
	public class ConsumersRepo
	{
		private readonly UlearnDb db;

		public ConsumersRepo(UlearnDb db)
		{
			this.db = db;
		}

		public LtiConsumer Find(string consumerKey)
		{
			return db.Consumers.SingleOrDefault(consumer => consumer.Key == consumerKey);
		}
	}
}