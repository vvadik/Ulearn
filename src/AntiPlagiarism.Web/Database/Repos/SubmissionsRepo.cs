using System;
using System.Threading.Tasks;
using AntiPlagiarism.Web.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AntiPlagiarism.Web.Database.Repos
{
	public interface ISubmissionsRepo
	{
		Task<Submission> GetSubmissionByIdAsync(int submissionId);
		Task<Submission> AddSubmissionAsync(int clientId, Guid taskId, Guid authorId, string code, string additionalInfo="");
	}

	public class SubmissionsRepo : ISubmissionsRepo
	{
		private readonly AntiPlagiarismDb db;

		public SubmissionsRepo(AntiPlagiarismDb db)
		{
			this.db = db;
		}

		public Task<Submission> GetSubmissionByIdAsync(int submissionId)
		{
			return db.Submissions.FirstOrDefaultAsync(s => s.Id == submissionId);
		}

		public async Task<Submission> AddSubmissionAsync(int clientId, Guid taskId, Guid authorId, string code, string additionalInfo="")
		{
			var submission = new Submission
			{
				ClientId = clientId,
				TaskId = taskId,
				AuthorId = authorId,
				Program = new Code
				{
					Text = code,
				},
				AdditionalInfo = additionalInfo,
				AddingTime = DateTime.Now,
			};

			await db.Submissions.AddAsync(submission);
			await db.SaveChangesAsync();

			return submission;
		}
	}
}