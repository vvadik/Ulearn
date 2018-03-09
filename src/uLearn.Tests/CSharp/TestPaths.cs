﻿using System;
using System.IO;

namespace uLearn.CSharp
{
	public static class TestPaths
	{
		public static string TestDirectoryPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..",
			"..", "CSharp");
		public static string BasicProgrammingDirectoryPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..",
			"..", "CSharp", "ExampleFiles", "BasicProgramming-master");
		public static string ULearnSubmissionsDirectoryPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..",
			"..", "CSharp", "ExampleFiles", "submissions");
	}
}