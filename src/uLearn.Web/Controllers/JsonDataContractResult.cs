using System;
using System.Runtime.Serialization.Json;
using System.Web.Mvc;

namespace uLearn.Web.Controllers
{
	public class JsonDataContractResult : JsonResult
	{
		public override void ExecuteResult(ControllerContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}
			if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
				string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Get is not allowed");
			}

			var response = context.HttpContext.Response;

			response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
			if (ContentEncoding != null)
				response.ContentEncoding = ContentEncoding;
			
			if (Data != null)
			{
				// Use the DataContractJsonSerializer instead of the JavaScriptSerializer 
				var serializer = new DataContractJsonSerializer(Data.GetType());
				serializer.WriteObject(response.OutputStream, Data);
			}
		}
	}
}