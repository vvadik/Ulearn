using System;
using System.IO;

namespace uLearn.Courses.BasicProgramming.Slides.U11_OOP
{
	[Slide("DirectoryInfo, FileInfo", "F74DCAFF-378C-43AE-B07B-AD18456C9153")]
	public class S050_DirectoryAndFile
	{
		//#video wIcfbRquD9k
		/*
		## Заметки по лекции
		*/
		static void Main()
		{
			foreach (var file in Directory.GetFiles("."))
				Console.WriteLine(file);

			Console.WriteLine(Directory.GetParent("."));

			var directoryInfo = new DirectoryInfo(".");
			foreach (var file in directoryInfo.GetFiles())
				Console.WriteLine(file.Name);
			directoryInfo = directoryInfo.Parent;
			Console.WriteLine(directoryInfo.FullName);
		}
	}
}