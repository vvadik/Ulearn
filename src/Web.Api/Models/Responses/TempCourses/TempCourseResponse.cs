using System;
using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Responses.TempCourses
{
	[DataContract]
	public class TempCourseResponse
	{
		[DataMember]
		public bool HasTempCourse;

		[DataMember]
		public string MainCourseId;

		[DataMember]
		public string TempCourseId;

		[DataMember]
		public DateTime LastUploadTime;

		[DataMember]
		public string Errors;
	}
}