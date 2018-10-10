using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos
{
	public interface IConsumersRepo
	{
		Task<LtiConsumer> FindAsync(string consumerKey);
	}
}