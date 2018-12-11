using System.Runtime.Serialization;
using Ulearn.Common;

namespace Ulearn.Web.Api.Models.Common
{
	[DataContract]
	public class ShortUserInfo
	{
		[DataMember(Name = "id")]
		public string Id { get; set; }
		
		[DataMember(Name = "login", EmitDefaultValue = false)]
		public string Login { get; set; }
		
		[DataMember(Name = "email", EmitDefaultValue = false)]
		public string Email { get; set; }
		
		[DataMember(Name = "first_name")]	
		public string FirstName { get; set; }
		
		[DataMember(Name = "last_name")]	
		public string LastName { get; set; }
		
		[DataMember(Name = "visible_name")]
		public string VisibleName { get; set; }

		[DataMember(Name = "avatar_url")]
		public string AvatarUrl { get; set; }

		[DataMember(Name = "gender")]
		public Gender? Gender { get; set; }
	}
}