using System;
using System.Threading.Tasks;
using System.Transactions;
using AntiPlagiarism.Web.Database.Models;
using Ulearn.Common.Extensions;

namespace AntiPlagiarism.Web.Database.Repos
{
	public interface IMostSimilarSubmissionsRepo
	{
		Task SaveMostSimilarSubmissionAsync(MostSimilarSubmission mostSimilarSubmission);
	}

	public class MostSimilarSubmissionsRepo : IMostSimilarSubmissionsRepo
	{
		private readonly AntiPlagiarismDb db;

		public MostSimilarSubmissionsRepo(AntiPlagiarismDb db)
		{
			this.db = db;
		}
		
		public async Task SaveMostSimilarSubmissionAsync(MostSimilarSubmission mostSimilarSubmission)
		{
			using (var ts = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(30), TransactionScopeAsyncFlowOption.Enabled))
			{
				db.AddOrUpdate(mostSimilarSubmission, p => p.SubmissionId == mostSimilarSubmission.SubmissionId);
				await db.SaveChangesAsync().ConfigureAwait(false);
				ts.Complete();
			}
		}
	}
}