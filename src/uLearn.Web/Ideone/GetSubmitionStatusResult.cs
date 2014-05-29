using System.Collections.Generic;

namespace uLearn.Web.Ideone
{
	public class GetSubmitionStatusResult : ResponseResult
	{
		public GetSubmitionStatusResult(Dictionary<string, string> response) : base(response)
		{
		}

		public SubmitionStatus Status
		{
			get { return (SubmitionStatus)int.Parse(Get("status")); }
		}

		public SubmitionResult Result
		{
			get { return (SubmitionResult)int.Parse(Get("result")); }
		}
	}
}