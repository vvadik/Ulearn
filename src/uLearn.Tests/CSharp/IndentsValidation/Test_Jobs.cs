using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace uLearn.CSharp
{
	[TestFixture]
	public class Test_Jobs
	{
		/// <summary>
		/// Локальный путь до директории с курсами
		/// </summary>
		DirectoryInfo courseDir = new DirectoryInfo("d:\\_work\\BasicProgramming\\");

		[Test(Description = "Вспомогательный метод для теста IndentsValidator_Should.NotFindErrors_On_Course_Cs_Files")]
		[Explicit]
		public void Write_All_Cs_FileFullNames_Of_Course_ToTxt()
		{
			var testFilepathsTxt = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilepaths.txt"));
			var result = courseDir.GetFiles("*.cs", SearchOption.AllDirectories)
				.Where(f => !f.Name.Equals("Settings.Designer.cs") && !f.Name.Equals("Resources.Designer.cs"))
				.Aggregate("", (current, file) => current + file.FullName + "\r\n");
			File.WriteAllText(testFilepathsTxt.FullName, result);
		}
	}
}