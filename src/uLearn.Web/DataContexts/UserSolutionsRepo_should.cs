using System;
using NUnit.Framework;

namespace uLearn.Web.DataContexts
{
	[TestFixture]
	public class UserSolutionsRepo_should
	{
		[Test]
		[Explicit]
		public void save_anonymous_users_solution()
		{
			var repo = new UserSolutionsRepo();
			var userSolution = repo.AddUserSolution("Linq", "0", "code", true, "", "output", null).Result;
			Console.WriteLine(userSolution.Id);
			repo.Delete(userSolution);
		}
	}
}