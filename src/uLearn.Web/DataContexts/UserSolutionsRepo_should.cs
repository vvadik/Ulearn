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
			var submission = repo.AddUserExerciseSubmission("Linq", Guid.Empty, "code", true, "", "output", null, null, "web").Result;
			Console.WriteLine(submission.Id);
			repo.DeleteSubmission(submission);
		}
	}
}