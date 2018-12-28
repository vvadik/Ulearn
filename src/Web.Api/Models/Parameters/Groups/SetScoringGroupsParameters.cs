using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Ulearn.Common.Api.Models.Parameters;
using Ulearn.Web.Api.Models.Binders;

namespace Ulearn.Web.Api.Models.Parameters.Groups
{
	[DataContract]
	[ModelBinder(typeof(JsonModelBinder), Name="parameters")]
	public class SetScoringGroupsParameters : ApiParameters
	{
		[DataMember(Name = "scores", IsRequired = true)]
		public List<string> ScoringGroupIds { get; set; }
	}
}