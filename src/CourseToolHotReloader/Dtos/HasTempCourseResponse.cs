using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CourseToolHotReloader.Dtos
{
	[DataContract]
	public class HasTempCourseResponse
	{
		[JsonPropertyName("hasTempCourse")]
		public bool HasTempCourse { get; set; }

		[JsonPropertyName("mainCourseId")]
		public string MainCourseId { get; set; }

		[JsonPropertyName("tempCourseId")]
		public string TempCourseId { get; set; }

		[JsonPropertyName("lastUploadTime")]
		public DateTime LastUploadTime { get; set; }
	}
}