using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

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

		public override string ToString()
		{
			return string.Join(Environment.NewLine, response.Select(kv => kv.Key + ": " + kv.Value));
		}
	}
}