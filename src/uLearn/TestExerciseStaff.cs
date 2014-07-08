using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace uLearn
{
	public class TestExerciseStaff
	{
		public static void TestExercise(MethodInfo methodInfo)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			var newOut = new StringWriter();
			Console.SetOut(newOut);
			methodInfo.Invoke(null, null);
			var attr =
				(ExpectedOutputAttribute)methodInfo.GetCustomAttributes(typeof(ExpectedOutputAttribute)).First();
			Assert.AreEqual(attr.Output, newOut.ToString().Trim());
		}
	}
}
