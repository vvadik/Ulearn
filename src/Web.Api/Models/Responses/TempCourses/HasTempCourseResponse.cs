using System;
using System.Runtime.Serialization;
using Database.Models;

namespace Ulearn.Web.Api.Models.Responses.TempCourses
{
	[DataContract]
	public class HasTempCourseResponse
	{
		[DataMember]
		public bool HasTempCourse;

		[DataMember]
		public string MainCourseId;

		[DataMember]
		public string TempCourseId;

		[DataMember]
		public DateTime LastUploadTime;
	}
}