using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Ulearn.Web.Api.Models.Binders;
using Ulearn.Web.Api.Models.Validations;

namespace Ulearn.Web.Api.Models.Parameters.Groups
{
	[DataContract]
	[ModelBinder(typeof(JsonModelBinder), Name="parameters")]
	public class CreateGroupParameters
	{
		[DataMember(Name = "name")]
		[NotEmpty(ErrorMessage = "Group name can not be empty")]
		public string Name { get; set; }
	}
}