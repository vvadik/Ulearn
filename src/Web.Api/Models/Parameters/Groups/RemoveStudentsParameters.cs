using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Ulearn.Web.Api.Models.Binders;

namespace Ulearn.Web.Api.Models.Parameters.Groups
{
	[DataContract]
	public class RemoveStudentsParameters
	{
		[DataMember(Name = "student_ids", IsRequired = true)]
		public List<string> UserIds { get; set; }
	}
}