using System;
using System.Runtime.Serialization;

namespace Ulearn.Core
{
	// Это данные, которые добавляются на сервере а автору курса никогда не видны
	[DataContract]
	public class CourseMeta
	{
		[DataMember]
		public Guid? Version { get; set; } // null для временных курсов и проверяемых курсов, для которых еще нет версии

		[DataMember]
		public DateTime? LoadingTime { get; set; } // То, что вместо версии во временном курсе, дата LoadingTime

		[DataMember]
		public bool IsTempCourse { get; set; }
	}
}