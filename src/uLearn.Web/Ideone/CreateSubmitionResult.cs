using System.Collections.Generic;

namespace uLearn.Web.Ideone
{
	public class CreateSubmitionResult : ResponseResult
	{
		public CreateSubmitionResult(Dictionary<string, string> response) : base(response)
		{
		}

		public string Link
		{
			get { return Get("link"); }
		}
	}
}