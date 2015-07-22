using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;
using ObjectPrinter;
using uLearn;

namespace uLearnToEdx
{
	[TestFixture]
	public class Converter_should
	{
		[Test, UseReporter(typeof(DiffReporter))]
		public void save_files()
		{
			var cm = new CourseManager(new DirectoryInfo(@"..\..\..\uLearn.Web"));
			cm.ReloadCourse("ForTests.zip");
			var course = cm.GetCourses().Single();
			var edxCourse = Converter.ToEdxCourse(course, "", new string[0], new string[0], "host");
			edxCourse.Save();
			//			ObjectPrinter.ObjectExtensions.
			Approvals.Verify(edxCourse.Dump(new ObjectPrinterConfig() { MaxDepth = 11 }));
		}
	}
}
