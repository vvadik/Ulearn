using System.Collections.Generic;

namespace uLearn.Web.Ideone
{
	public class ResponseResult
	{
		private readonly Dictionary<string, string> response;

		public ResponseResult(Dictionary<string, string> response)
		{
			this.response = response;
		}

		public bool IsOk
		{
			get { return Error == "OK"; }
		}

		public string Error
		{
			get { return Get("error"); }
		}

		protected string Get(string key)
		{
			return response[key];
		}
	}
}