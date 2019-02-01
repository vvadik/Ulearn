using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault.Models;
using Ulearn.Common.Api.Models.Validations;
using Ulearn.Core.Courses;
using Ulearn.Web.Api.Authorization;
using Ulearn.Web.Api.Models.Binders;

namespace Ulearn.Web.Api.Models.Parameters.Groups
{
	[DataContract]
	[ModelBinder(typeof(JsonModelBinder), Name="parameters")]
	public class CreateGroupParameters : ICourseAuthorizationParameters
	{
		[DataMember(Name = "course_id", IsRequired = true)]
		public string CourseId { get; set; }
		
		[DataMember(Name = "name", IsRequired = true)]
		[NotEmpty(ErrorMessage = "Group name can not be empty")]
		public string Name { get; set; }
	}
}