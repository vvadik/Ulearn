using System;

namespace Ulearn.Web.Api.Controllers
{
	public class StatusCodeException : Exception
	{
		public int Code { get; }

		public StatusCodeException(int code, string message) : base(message)
		{
			Code = code;
		}
	}
}