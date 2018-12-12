using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Repos.Users;
using Database.Repos.Users.Search;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Ulearn.Common;

namespace Database.Core.Tests.Repos.Users
{
	[TestFixture]
	public class UserSearcherTests : BaseRepoTests
	{
		private UserSearcher userSearcher;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			userSearcher = new UserSearcher(db, serviceProvider.GetServices<ISearcher>(), serviceProvider.GetServices<IFilter>());
		}

		[Test]
		public async Task SearchByUserId()
		{
			var user = await CreateUserAsync("test").ConfigureAwait(false);
			
			var result = await userSearcher.SearchUsersAsync(new UserSearchRequest
			{
				Words = new List<string> { user.Id.Substring(0, 5) }
			}).ConfigureAwait(false);
			
			Assert.AreEqual(1, result.Count);
		}
		
		[Test]
		public async Task SearchByUserIdNotFoundUnnecessary()
		{
			var user1 = await CreateUserAsync("test1").ConfigureAwait(false);
			var user2 = await CreateUserAsync("test2").ConfigureAwait(false);

			if (user1.Id.Substring(0, 5) == user2.Id.Substring(0, 5))
				return;
			
			var result = await userSearcher.SearchUsersAsync(new UserSearchRequest
			{
				Words = new List<string> { user1.Id.Substring(0, 5) }
			}).ConfigureAwait(false);
			
			Assert.AreEqual(1, result.Count);
		}
		
		[Test]
		[Explicit("Слишком долгий тест, чтобы запускаться обязательно. Может, переписать на явное указание Id при создании пользователя?")]
		public async Task SearchByUserIdFindsAll()
		{
			var user1 = await CreateUserAsync("test1").ConfigureAwait(false);

			var user2 = await CreateUserAsync("test2").ConfigureAwait(false);
			while (user1.Id.Substring(0, 3) != user2.Id.Substring(0, 3))
				user2 = await CreateUserAsync("test" + StringUtils.GenerateAlphanumericString(10)).ConfigureAwait(false);
			
			var result = await userSearcher.SearchUsersAsync(new UserSearchRequest
			{
				Words = new List<string> { user1.Id.Substring(0, 3) }
			}).ConfigureAwait(false);
			
			Assert.AreEqual(2, result.Count);
		}

		/*
		[Test]
		public async Task SearchByName()
		{
			var user = await CreateUserAsync("test").ConfigureAwait(false);
			user.FirstName = "Abacaba";
			user.LastName = "Qwerty";
			await db.SaveChangesAsync().ConfigureAwait(false);

			var result = await userSearcher.SearchUsersAsync(new UserSearchRequest
			{
				Words = new List<string> { "aba" }
			}).ConfigureAwait(false);

			Assert.AreEqual(1, result.Count);
			
			result = await userSearcher.SearchUsersAsync(new UserSearchRequest
			{
				Words = new List<string> { "abaq" }
			}).ConfigureAwait(false);
			Assert.AreEqual(0, result.Count);
		}*/
		
	}
}