using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ulearn.Web.Api.Swagger
{
	public class BadRequestResponseOperationFilter : IOperationFilter
	{
		public void Apply(Operation operation, OperationFilterContext context)
		{
			if (context.ApiDescription.ParameterDescriptions.Any())
				operation.Responses.Add("400", new Response { Description = "Bad request. Invalid parameters in url or body" });
		}
	}
}