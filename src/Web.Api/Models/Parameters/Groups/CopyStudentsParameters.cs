using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Ulearn.Common.Api.Models.Validations;
using Ulearn.Web.Api.Models.Binders;

namespace Ulearn.Web.Api.Models.Parameters.Groups
{
	[DataContract]
	[ModelBinder(typeof(JsonModelBinder), Name="parameters")]
	public class CopyStudentsParameters
	{
		[DataMember(Name = "destination_group_id", IsRequired = true)]
		[MinValue(0)]
		public int DestinationGroupId { get; set; }
		
		[DataMember(Name = "student_ids", IsRequired = true)]
		public List<string> UserIds { get; set; }
	}
}