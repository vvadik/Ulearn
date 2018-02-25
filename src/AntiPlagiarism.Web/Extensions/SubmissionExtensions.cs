using AntiPlagiarism.Api.Models.Results;
using AntiPlagiarism.Web.Database.Models;

namespace AntiPlagiarism.Web.Extensions
{
	public static class SubmissionExtensions
	{
		public static SubmissionInfo GetSubmissionInfoForApi(this Submission submission)
		{
			return new SubmissionInfo
			{
				Id = submission.Id,
				TaskId = submission.TaskId,
				AuthorId = submission.AuthorId,
				Code = submission.ProgramText,
				AdditionalInfo = submission.AdditionalInfo,
			};
		}
	}
}