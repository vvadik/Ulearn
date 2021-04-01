using System.Linq;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos.Users.Search
{
	public class SearcherByEmail : ISearcher
	{
		private readonly UlearnDb db;

		public SearcherByEmail(UlearnDb db)
		{
			this.db = db;
		}

		public SearchField GetSearchField()
		{
			return SearchField.UserId;
		}

		public Task<IQueryable<ApplicationUser>> GetSearchScopeAsync(IQueryable<ApplicationUser> users, ApplicationUser currentUser, string courseId)
		{
			return Task.FromResult(users);
		}

		public Task<bool> IsAvailableForSearchAsync(ApplicationUser currentUser)
		{
			return Task.FromResult(true);
		}

		public Task<IQueryable<ApplicationUser>> SearchAsync(IQueryable<ApplicationUser> users, string term, bool strict = false, int limit = 1000)
		{
			/* Don't search if not email */
			if (!term.Contains("@") || !term.Contains("."))
				return Task.FromResult(Enumerable.Empty<ApplicationUser>().AsQueryable());

			/* This searcher works identically in strict and non-strict mode */
			return Task.FromResult(users.Where(u => u.Email == term));
		}
	}
}