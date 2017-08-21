using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace uLearn.CSharp
{
	[TestFixture]
	public class Test_Jobs
	{
		DirectoryInfo courseDir = new DirectoryInfo("d:\\_work\\BasicProgramming\\");

		[Test]
		[Explicit]
		public void Write_All_Cs_FileFullNames_Of_Course_ToTxt()
		{
			var testFilepathsTxt = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFilepaths.txt"));
			var result = courseDir.GetFiles("*.cs", SearchOption.AllDirectories)
				.Aggregate("", (current, file) => current + file.FullName + "\r\n");
			File.WriteAllText(testFilepathsTxt.FullName, result);
		}

		[Test]
		[Explicit]
		public void Fix_Programs()
		{
			var programs = courseDir.GetDirectories("OOP").Single().GetFiles("Program.cs", SearchOption.AllDirectories);
			foreach (var program in programs)
			{
				File.WriteAllText(program.FullName, @"using System.Collections.Generic;
using System.Linq;
using NUnitLite;

class Program
{
	static void Main(string[] args)
	{
		new AutoRun().Execute(args);
	}
}
");
			}
		}
	}
}