using System.Threading.Tasks;
using Database.Repos;
using Database.Repos.Groups;
using Moq;
using NUnit.Framework;

namespace Database.Core.Tests.Repos
{
	public class GoogleSheetRepoTests : BaseRepoTests
	{
		private GoogleSheetExportTasksRepo googleSheetExportTasksRepo;
		
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			googleSheetExportTasksRepo = new GoogleSheetExportTasksRepo(db);
		}

		[Test]
		public async Task GetIds()
		{
			// var f = await GoogleSheetTasksRepo.GetGroupsIds();
		}

	}
}