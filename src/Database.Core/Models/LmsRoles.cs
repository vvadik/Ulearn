using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Database.Models
{
	[JsonConverter(typeof(StringEnumConverter), true)]
	public enum LmsRoles 
	{
		[Display(Name = "Сис. админ")]
		SysAdmin
	}

	[JsonConverter(typeof(StringEnumConverter), true)]
	public enum CourseRole
	{
		[Display(Name = "Администратор курса")]
		CourseAdmin,

		[Display(Name = "Преподаватель")]
		Instructor,

		[Display(Name = "Тестер")]
		Tester,

		[Display(Name = "Студент")]
		Student
	}
}