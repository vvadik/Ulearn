using System.Threading.Tasks;
using Database.Repos;
using Database.Repos.Groups;
using Moq;
using NUnit.Framework;

namespace Database.Core.Tests.Repos
{
	public class GoogleSheetRepoTests : BaseRepoTests
	{
		private GoogleSheetTasksRepo GoogleSheetTasksRepo;
		
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			GoogleSheetTasksRepo = new GoogleSheetTasksRepo(db);
		}

		[Test]
		public async Task GetIds()
		{
			var f = await GoogleSheetTasksRepo.GetGroupsIds();
		}

	}
}