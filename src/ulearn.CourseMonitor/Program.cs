using System;
using uLearn.CourseTool;

namespace ulearn.CourseMonitor
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				Console.WriteLine(@"Failed to start monitor. 2 parameters required. Do not start monitor directly, use `course` tool");
			}
			Monitor.Start(args[0], args[1]);
		}
	}
}