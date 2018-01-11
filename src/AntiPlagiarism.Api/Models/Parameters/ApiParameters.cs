using AntiPlagiarism.Api.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace AntiPlagiarism.Api.Models.Parameters
{
	[ModelBinder(typeof(JsonModelBinder), Name="parameters")]
	public abstract class ApiParameters
	{
	}
}