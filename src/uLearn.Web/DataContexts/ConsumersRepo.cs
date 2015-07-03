using System.Linq;
using uLearn.Web.Models;

namespace uLearn.Web.DataContexts
{
	internal class ConsumersRepo
	{
		private readonly ULearnDb db;

		public ConsumersRepo()
			: this(new ULearnDb())
		{

		}

		public ConsumersRepo(ULearnDb db)
		{
			this.db = db;
		}

		public Consumer Find(string consumerKey)
		{
			return db.Consumers.SingleOrDefault(consumer => consumer.Key == consumerKey);
		}

	}
}