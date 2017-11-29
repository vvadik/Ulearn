using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using uLearn.Extensions;

namespace uLearn.CSharp
{
	[TestFixture]
	public class NamingLengthValidator_should
	{
		private static readonly DirectoryInfo testDataDir = new DirectoryInfo(Path.Combine(@"C:\Users\smprivalov\Downloads\BasicProgramming-master\BasicProgramming-master"));
		//private static readonly DirectoryInfo testDataDir = new DirectoryInfo(Path.Combine(@"C:\Users\smprivalov\Downloads\ulearn.submissions\submissions"));

		private static IEnumerable<FileInfo> files = testDataDir.GetFiles("*.cs.", SearchOption.AllDirectories).Take(4000);

		[Test]
		[TestCaseSource(nameof(files))]
		public void FindErrors(FileInfo fileInfo)
		{
			Console.WriteLine(fileInfo.FullName);
			var errors = new NamimgLengthValidator().FindError(fileInfo.ContentAsUtf8());
			if (errors != null)
			{
				Console.WriteLine(errors);
			}
			errors.Should().BeNullOrEmpty();
		}

		
		[TestCase(@"void SomeMethod(int a){}")]
		[TestCase(@"void A(string s){}")]
		[TestCase(@"public int DrawingPriority() => 0;")]
		public void warn_method_name_without_verb(string code)
		{
			FindErrors(code).Should().NotBeNullOrEmpty();
		}

		[TestCase(@"bool SetX(int x){ return true; }")]
		[TestCase(@"int GetX(){ return 1; }")]
		[TestCase(@"bool IsCorrectMethod(){ return true; }")]
		[TestCase(@"public int GetDrawingPriority() => 0;")]
		public void ignore_correct_method_name(string code)
		{
			FindErrors(code).Should().BeNullOrEmpty();
		}

		[TestCase(@"public static Rational operator *(Rational r){}")]
		[TestCase(@"public static Rational operator +(Rational r){}")]
		[TestCase(@"public static Rational operator double(Rational r){}")]
		[TestCase(@"public static Rational operator int(Rational r){}")]
		public void ignore_operator(string code)
		{
			FindErrors(code).Should().BeNullOrEmpty();
		}

		[TestCase(@"public class SomeClass{public SomeClass(){ }}")]
		[TestCase(@"public class Point{public Point(){ }}")]
		public void ignore_costructors(string code)
		{
			FindErrors(code).Should().BeNullOrEmpty();
		}

		private static string FindErrors(string code) =>
			new NamimgLengthValidator().FindError(code);
	}
}