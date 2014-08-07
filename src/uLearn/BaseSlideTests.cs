using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using NUnit.Framework;

namespace uLearn
{
	[TestFixture]
	public abstract class BaseSlideTests
	{
		private readonly Type someSlideClass;

		protected BaseSlideTests(Type someSlideClass)
		{
			this.someSlideClass = someSlideClass;
		}

		[Test]
		public void TestGetAllSlides()
		{
			var slides = GetExerciseSlidesTestCases();
			foreach (var slide in slides)
			{
				Console.WriteLine(slide.TestName);
			}
		}

		[TestCaseSource("GetExerciseSlidesTestCases")]
		public void Slide(Type slideType)
		{
			TestExercise(slideType);
		}

		[Test]
		public void TestUniqueSlideIds()
		{
			var ids = new HashSet<string>();
			foreach (var slideType in GetSlideTypes())
			{
				Assert.IsTrue(ids.Add(slideType.Item2.Guid), slideType.Item1 + " duplicate slide id?");
			}
		}

		public IEnumerable<TestCaseData> GetExerciseSlidesTestCases()
		{
			return GetSlideTypes()
				.Select(type_attr => type_attr.Item1)
				.Where(type => GetExpectedOutputAttributes(type).Any())
				.Select(type => new TestCaseData(type).SetName(type.Name + ".Main"));
		}

		public IEnumerable<Tuple<Type, SlideAttribute>> GetSlideTypes()
		{
			return someSlideClass.Assembly
				.GetTypes()
				.Where(t => t.Namespace == someSlideClass.Namespace)
				.Select(t => Tuple.Create(t, t.GetCustomAttributes(typeof(SlideAttribute)).Cast<SlideAttribute>().FirstOrDefault()))
				.Where(t => t.Item2 != null);
		}

		public static void TestExercise(Type slide)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			var newOut = new StringWriter();
			var oldOut = Console.Out;
			var methodInfo = slide.GetMethod("Main", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			Console.SetOut(newOut);
			try
			{
				methodInfo.Invoke(null, null);
			}
			finally
			{
				Console.SetOut(oldOut);
			}
			var declaringType = methodInfo.DeclaringType;
			if (declaringType == null) throw new Exception("should be!");
			var expectedOutput = String.Join("\n", GetExpectedOutputAttributes(declaringType).Select(a => a.Output));
			Console.WriteLine(newOut.ToString());
			Assert.AreEqual(PrepareOutput(expectedOutput), PrepareOutput(newOut.ToString()));
		}

		private static string PrepareOutput(string output)
		{
			return String.Join("\n", output.Trim().SplitToLines());
		}

		public static IEnumerable<ExpectedOutputAttribute> GetExpectedOutputAttributes(Type declaringType)
		{
			return declaringType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
				.SelectMany(m => m.GetCustomAttributes(typeof(ExpectedOutputAttribute)))
				.Cast<ExpectedOutputAttribute>();
		}
	}
}