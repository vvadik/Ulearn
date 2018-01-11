using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using AntiPlagiarism.Web.Controllers.ModelBinders;
using AntiPlagiarism.Web.Database;
using AntiPlagiarism.Web.Database.Repos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;

namespace AntiPlagiarism.Web.Controllers
{
	[Route("/Api")]
	public class ApiController : BaseController
	{
		private readonly ISubmissionsRepo submissionsRepo;

		public ApiController(ISubmissionsRepo submissionsRepo, ILogger logger)
			: base(logger)
		{
			this.submissionsRepo = submissionsRepo;
		}
		
		[HttpPost("AddCode")]
		public async Task<IActionResult> AddSubmission(AddCodeApiParams parameters)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var submission = await submissionsRepo.AddSubmissionAsync(client.Id, parameters.TaskId, parameters.AuthorId, parameters.Code, parameters.AdditionalInfo);
			
			return Json(new AddCodeApiResult
			{
				SubmissionId = submission.Id,
			});
		}
	}

	[ModelBinder(typeof(JsonModelBinder), Name="parameters")]
	public abstract class ApiParams
	{
	}
	
	[DataContract]
	public class AddCodeApiParams : ApiParams
	{
		[DataMember(Name = "task_id", IsRequired = true)]
		public Guid TaskId { get; set; }
		
		[DataMember(Name = "author_id", IsRequired = true)]
		public Guid AuthorId { get; set; }
		
		[DataMember(Name = "code", IsRequired = true)]
		public string Code { get; set; }
		
		[DataMember(Name = "additional_info")]
		public string AdditionalInfo { get; set; }

		public override string ToString()
		{
			return $"AddCodeParams(TaskId={TaskId}, AuthorId={AuthorId}, Code={Code?.Substring(0, Math.Min(Code.Length, 50))?.Replace("\n", @"\\")}...)";
		}
	}

	public enum ApiResultStatus
	{
		Ok = 1,
		Error = 2,
	}
	
	[DataContract]
	public abstract class ApiResult
	{
		[DataMember(Name = "status")]
		[JsonConverter(typeof(StringEnumConverter), true)]
		public ApiResultStatus Status { get; set; }
	}

	[DataContract]
	public class ApiError : ApiResult
	{
		[DataMember(Name = "error")]
		public string Error { get; set; }

		public static ApiError Create(string error)
		{
			return new ApiError
			{
				Status = ApiResultStatus.Error,
				Error = error,
			};
		}
	}

	[DataContract]
	public class ApiSuccessResult : ApiResult
	{
		protected ApiSuccessResult()
		{
			Status = ApiResultStatus.Ok;
		}
	}
	
	[DataContract]
	public class AddCodeApiResult : ApiSuccessResult
	{
		[DataMember(Name = "submission_id")]
		public int SubmissionId { get; set; }
	}
}