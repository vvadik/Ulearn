using System;
using System.Threading.Tasks;
using AntiPlagiarism.Api.Models;
using AntiPlagiarism.Web.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AntiPlagiarism.Web.Database.Repos
{
	public interface ISubmissionsRepo
	{
		Task<Submission> GetSubmissionByIdAsync(int submissionId);
		Task<Submission> AddSubmissionAsync(int clientId, Guid taskId, Guid authorId, Language language, string code, string additionalInfo="");
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

		public async Task<Submission> AddSubmissionAsync(int clientId, Guid taskId, Guid authorId, Language language, string code, string additionalInfo="")
		{
			var submission = new Submission
			{
				ClientId = clientId,
				TaskId = taskId,
				AuthorId = authorId,
				Language = language,
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