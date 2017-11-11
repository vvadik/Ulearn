using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using uLearn.Extensions;

namespace uLearn.CSharp
{
	[TestFixture]
	public class NamingLengthValidator_should
	{
		private NamimgLengthValidator validator;

		[SetUp]
		public void SetUp()
		{
			validator = new NamimgLengthValidator();
		}

		private static readonly DirectoryInfo testDataDir = new DirectoryInfo(Path.Combine(@"C:\Users\smprivalov\Downloads\BasicProgramming-master\BasicProgramming-master"));

		private static IEnumerable<FileInfo> files = testDataDir.GetFiles("*.Solution.cs.", SearchOption.AllDirectories);

		[Test]
		[TestCaseSource(nameof(files))]
		public void FindErrors(FileInfo fileInfo)
		{
			Console.WriteLine(fileInfo.FullName);
			var errors = validator.FindError(fileInfo.ContentAsUtf8());
			if (errors != null)
			{
				Console.WriteLine(errors);
			}
			errors.Should().BeNullOrEmpty();
		}
		
		[TestCase(@"public class SomeClass{public readonly int N {get;}")]
		[TestCase(@"public class SomeClass{private int A { get; set; }}")]
		[TestCase(@"public class SomeClass{public int N { get; set; }}")]
		public void warn_class_properties_with_short_name(string code)
		{
			var errors = FindErrors(code);
			Assert.That(errors, Is.Not.Null);
		}
		
		[TestCase(@"public class SomeClass{public readonly int X {get;}")]
		[TestCase(@"public class SomeClass{private int Y { get; set; }}")]
		[TestCase(@"public class SomeClass{public int Z { get; set; }}")]
		public void ignore_class_properties_with_valid_name(string code)
		{
			var errors = FindErrors(code);
			Assert.That(errors, Is.Null);
		}

		[TestCase(@"public class SomeClass{public readonly int N;}")]
		[TestCase(@"public class SomeClass{int A;}")]
		public void warn_class_fields_with_short_name(string code)
		{
			var errors = FindErrors(code);
			Assert.That(errors, Is.Not.Null);
		}

		[TestCase(@"public class SomeClass{public readonly int X;}")]
		[TestCase(@"public class SomeClass{int Y;}")]
		[TestCase(@"public class SomeClass{public double Z;}")]
		[TestCase(@"public class SomeClass{public const double g;}")]
		public void ignore_correct_class_fields_with_short_name_and_constants(string code)
		{
			var errors = FindErrors(code);
			Assert.That(errors, Is.Null);
		}

		[TestCase(@"void SomeMethod(int a){}")]
		[TestCase(@"void SomeMethod(Point p){}")]
		[TestCase(@"public class SomeClass{ public SomeClass(Expression e){}}")]
		[TestCase(@"public static implicit operator double(Rational r){}")]
		[TestCase(@"public static Rational operator *(Rational r){}")]
		public void warn_method_arguments_with_short_name(string code)
		{
			var errors = FindErrors(code);
			Assert.That(errors, Is.Not.Null);
		}

		[TestCase(@"bool SomeMethod(int x, int y, int z){ return true; }")]
		[TestCase(@"bool SomeMethod(int numberOfErrors){ return true; }")]
		[TestCase(@"bool SomeMethod(){ return true; }")]
		[TestCase(@"public int GetDrawingPriority() => 0;")]
		public void ignore_correct_arguments_name(string code)
		{
			var errors = FindErrors(code);
			Assert.That(errors, Is.Null);
		}
		
		[TestCase(@"bool SomeMethod(int x, int y, int z){ var b = true; return b; }")]
		[TestCase(@"void SomeMethod(int numberOfErrors){ int a; }")]
		public void warn_local_variables_short_name(string code)
		{
			var errors = FindErrors(code);
			Assert.That(errors, Is.Not.Null);
		}

		[Test]
		public void ignore_correct_local_variables_names()
		{
			var errors = FindErrors(@"void MethodWithcycle(){
var numberOfSmth = 0;
string somestr;
 for (; InstructionPointer < Instructions.Length; InstructionPointer++)
			{
				Action<IVirtualMachine> command;
				if (commands.TryGetValue(Instructions[InstructionPointer], out command))
					command(this);
			} }");
			Assert.That(errors, Is.Null);
		}

		[Test]
		public void find_all_warnings()
		{
			var errors = FindErrors(@"
void Method(int a){ if (true) return true; else return false; }
void Method(){ var b = 0; }
void Method(){ for (var p = 0; p < 1; ++p){} }
");
			Assert.That(errors, Does.Contain("Строка 2"));
			Assert.That(errors, Does.Contain("Строка 3"));
			Assert.That(errors, Does.Contain("Строка 4"));
		}

		private static string FindErrors(string code)
		{
			var errors = new NamimgLengthValidator().FindError(code);
			return errors;
		}
	}
}