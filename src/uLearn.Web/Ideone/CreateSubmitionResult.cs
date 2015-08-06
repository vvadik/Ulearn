using System;
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
			get
			{
				if (IsOk)
					return Get("link");
				throw new Exception(ToString());
			}
		}
	}
}