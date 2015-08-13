using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using RunCsJob;
using uLearn.Web.DataContexts;

namespace uLearn.Web.Controllers
{
	public class RunnerController : ApiController
	{
//		private readonly UserRepo _user = new UserRepo();
		private readonly UserSolutionsRepo _userSolutionsRepo = new UserSolutionsRepo();
		private readonly CourseManager courseManager = WebCourseManager.Instance;
		/// <summary>
		/// Return one submission for testing purposes
		/// </summary>
		/// <param name="token"> Runner autherization token </param>
		[HttpGet]
		[Route("TryGetSubmission")]
		public InternalSubmissionModel TryGetSubmission(string token)
		{
			CheckRunner(token);

			var submission = _userSolutionsRepo.FindUnhandled();
			if (submission == null)
				return null;
			return new InternalSubmissionModel
			{
				Id = submission.Id.ToString(),
				Code = submission.SolutionCode.Text,
				Input = "",
				NeedRun = true
			};
		}

		/// <summary>
		/// Return list of submissions for testing purposes
		/// </summary>
		/// <param name="token"> Runner autherization token </param>
		/// <param name="count"> Count of submission </param>
		[HttpGet]
		[Route("GetSubmissions")]
		public List<InternalSubmissionModel> GetSubmissions([FromUri] string token, [FromUri] int count)
		{
			CheckRunner(token);
			var submissions = _userSolutionsRepo.GetUnhandled(count);
			return submissions
				.Select(details => new InternalSubmissionModel
				{
					Id = details.Id.ToString(),
					Code = Utils.GetSource(
						details.CourseId, 
						details.SlideId, 
						courseManager, 
						details.SolutionCode.Text
					),
					Input = "",
					NeedRun = true
				})
				.ToList();
		}

		/// <summary>
		/// Get testing result
		/// </summary>
		/// <param name="token"> Runner autherization token </param>
		[HttpPost]
		[Route("PostResult")]
		public async Task PostResult([FromUri]string token, RunningResults result)
		{
			if (!ModelState.IsValid)
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			CheckRunner(token);

			await _userSolutionsRepo.SaveResults(result);
		}

		/// <summary>
		/// Get testing results
		/// </summary>
		/// <param name="token"> Runner autherization token </param>
		[HttpPost]
		[Route("PostResults")]
		public async Task PostResults([FromUri]string token, List<RunningResults> results)
		{
			if (!ModelState.IsValid)
				throw new HttpResponseException(HttpStatusCode.BadRequest);
			CheckRunner(token);

			await _userSolutionsRepo.SaveAllResults(results);
		}


		private void CheckRunner(string token)
		{
//			var roles = _user.FindRoles(token);
//			if (!roles.Contains(Role.Runner))
//				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
		}
	}
}
