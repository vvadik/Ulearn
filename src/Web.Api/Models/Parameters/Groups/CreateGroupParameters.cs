using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Ulearn.Common.Api.Models.Validations;
using Ulearn.Web.Api.Models.Binders;

namespace Ulearn.Web.Api.Models.Parameters.Groups
{
	[DataContract]
	[ModelBinder(typeof(JsonModelBinder), Name="parameters")]
	public class CreateGroupParameters
	{
		[DataMember(Name = "name", IsRequired = true)]
		[NotEmpty(ErrorMessage = "Group name can not be empty")]
		public string Name { get; set; }
	}
}