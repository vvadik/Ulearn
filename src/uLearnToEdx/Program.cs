using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApprovalUtilities.Utilities;
using NUnit.Framework;
using uLearn;
using uLearn.Model.Edx;

namespace uLearnToEdx
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var cm = new CourseManager(new DirectoryInfo(@"..\..\..\uLearn.Web"));
			cm.ReloadCourse("BasicProgramming.zip");
			var course = cm.GetCourses().Single();
			var converted = Converter.ToEdxCourse(course, "Kontur", new[] { "lti" }, new[] { "myname:rfe:qwerty" }, "192.168.33.1:44300", new Dictionary<string, string>());
			converted.Save(course.Id);
			ArchiveManager.CreateTar(course.Id + ".tar.gz", course.Id);
			
			EdxCourse
				.Load("BasicProgramming")
				.Save("BasicProgramming2");
			ArchiveManager.CreateTar("BasicProgramming2.tar.gz", "BasicProgramming2");
		}
	}
}
