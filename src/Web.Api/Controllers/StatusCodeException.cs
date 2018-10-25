using System;

namespace Ulearn.Web.Api.Controllers
{
	public class StatusCodeException : Exception
	{
		public int Code { get; }
		public object Result { get; }

		public StatusCodeException(int code, object result)
		{
			Code = code;
			Result = result;
		}
	}
}