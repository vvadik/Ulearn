using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using AntiPlagiarism.Api;
using AntiPlagiarism.Api.Models.Parameters;
using Database.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Ulearn.Core.RunCheckerJobApi;
using Web.Api.Configuration;

namespace Ulearn.Web.Api.Controllers.Runner
{
	public class AntiPlagiarismResultObserver : IResultObserver
	{
		private readonly IAntiPlagiarismClient antiPlagiarismClient;
		private readonly bool isEnabled;

		public AntiPlagiarismResultObserver(IOptions<WebApiConfiguration> configuration, ILogger logger)
		{
			var antiplagiarismClientConfiguration = configuration.Value.AntiplagiarismClient;
			isEnabled = antiplagiarismClientConfiguration?.Enabled ?? false;
			if (!isEnabled)
				return;

			antiPlagiarismClient = new AntiPlagiarismClient(antiplagiarismClientConfiguration.Endpoint, antiplagiarismClientConfiguration.Token, logger);
		}

		public async Task ProcessResult(UserExerciseSubmission submission, RunningResults result)
		{
			if (!isEnabled)
				return;

			if (result.Verdict != Verdict.Ok)
				return;

			/* Send to antiplagiarism service only accepted submissions */
			var checking = submission.AutomaticChecking;
			if (!checking.IsRightAnswer)
				return;

			var parameters = new AddSubmissionParameters
			{
				TaskId = submission.SlideId,
				Language = submission.Language,
				Code = submission.SolutionCode.Text,
				AuthorId = Guid.Parse(submission.UserId),
				AdditionalInfo = JsonConvert.SerializeObject(new AntiPlagiarismAdditionalInfo { SubmissionId = submission.Id }),
				ClientSubmissionId = submission.Id.ToString()
			};
			await antiPlagiarismClient.AddSubmissionAsync(parameters).ConfigureAwait(false);
		}
	}

	[DataContract]
	public class AntiPlagiarismAdditionalInfo
	{
		[DataMember]
		public int SubmissionId { get; set; }
	}
}