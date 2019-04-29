using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Ulearn.Web.Api.Models.Common
{
	[DataContract]
	public class ShortGroupInfo
	{
		[DataMember]
		public int Id { get; set; }
		
		[DataMember]
		public string Name { get; set; }
		
		[DataMember]
		public bool IsArchived { get; set; }

		[CanBeNull]
		[DataMember]
		public string ApiUrl { get; set; } // null, если нет прав на переход на страницу группы
	}
}