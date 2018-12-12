using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Models;
using Database.Repos.Users.Search;
using Ulearn.Common;

namespace Database.Repos.Users
{
	public class UserSearcher : IUserSearcher
	{
		private readonly UlearnDb db;
		private readonly List<ISearcher> searchers;
		private readonly List<IFilter> filters;

		public UserSearcher(UlearnDb db, IEnumerable<ISearcher> searchers, IEnumerable<IFilter> filters)
		{
			this.db = db;
			this.searchers = searchers.ToList();
			this.filters = filters.ToList();
		}

		public async Task<List<FoundUser>> SearchUsersAsync(UserSearchRequest request, bool strict = false, int limit = 50)
		{
			var result = db.Users.Where(u => !u.IsDeleted);

			var usersFields = new DefaultDictionary<string, HashSet<SearchField>>();
			
			foreach (var word in request.Words)
			{
				if (string.IsNullOrEmpty(word))
					continue;

				var currentSet = new HashSet<string>();

				foreach (var searcher in searchers)
				{
					var isAvailable = await searcher.IsAvailableForSearchAsync(request.CurrentUser).ConfigureAwait(false);
					if (isAvailable)
					{
						var scope = await searcher.GetSearchScopeAsync(result, request.CurrentUser).ConfigureAwait(false);
						var searcherResult = await searcher.SearchAsync(scope, word, strict).ConfigureAwait(false);

						var foundUserIds = searcherResult.Select(u => u.Id).ToList();
						var searchField = searcher.GetSearchField();
						foreach (var foundUserId in foundUserIds)
							usersFields[foundUserId].Add(searchField);
						
						currentSet.UnionWith(foundUserIds);
					}
				}

				result = result.Where(u => currentSet.Contains(u.Id));
			}
			
			foreach (var filter in filters)
				result = await filter.FilterAsync(result, request).ConfigureAwait(false);

			return result.OrderByDescending(u => usersFields[u.Id].Count).ThenBy(u => u.LastName).ThenBy(u => u.FirstName).Take(limit).Select(u => new FoundUser
			{
				User = u,
				Fields = usersFields[u.Id],
			}).ToList();
		}
	}

	public class UserSearchRequest
	{
		/* Current user is used to define access to some searchers */
		
		public ApplicationUser CurrentUser { get; set; }
		
		/* Search by words: */
		
		public List<string> Words { get; set; }
		
		/* and filter result by course and system roles: */
		
		public string CourseId { get; set; }
		
		public CourseRoleType? MinCourseRoleType { get; set; }
		
		public LmsRoleType? LmsRole { get; set; }
	}
}