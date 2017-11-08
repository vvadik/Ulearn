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
	public class ParametrNameLengthValidator_should
	{
		private ParametrNameLengthValidator validator;

		[SetUp]
		public void SetUp()
		{
			validator = new ParametrNameLengthValidator();
		}

		


		[TestCase(@"public static implicit operator double(Rational r1){}")]
		[TestCase(@"public static Rational operator *(Rational r1, Rational r2){}")]
		public void ignore_operator_method_name(string code)
		{
			var errors = FindErrors(code);
			Assert.That(errors, Is.Null);
		}

		[Test]
		public void ignore_parametr_name_if_it_is_coordinates_name()
		{
			var errors = FindErrors(@"bool SomeMethod(int x, int y, int z){ return true; }");
			Assert.That(errors, Is.Null);
		}

		[TestCase(@"bool A(){ if (true) return true; else return false; }")]
		[TestCase(@"bool a(){ if (true) return true; else return false; }")]
		public void warn_short_function_Names(string code)
		{
			var errors = FindErrors(code);
			Assert.That(errors, Is.Not.Null);
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

		[Test]
		public void warn_method_arguments_with_short_name()
		{
			var errors = FindErrors(@"int SomeMethod(int a){ if (true) return 1; else return 2; }");
			Assert.That(errors, Is.Not.Null);
		}

		[Test]
		public void warn_short_name_in_constructor()
		{
			var errors = FindErrors(@"public class SomeClass{ public SomeClass(int a){}}");
			Assert.That(errors, Is.Not.Null);
		}

		[Test]
		public void ignore_correct_fields_names()
		{
			var errors = FindErrors(@"
class SomeClass{
public int NumberOfSomething; 
string somestring;
bool SomeMethod(){ 
if (true) 
return f(); 
else 
return false; }}");
			Assert.That(errors, Is.Null);
		}

		[Test]
		public void ignore_short_variables_in_cycle()
		{
			var errors = FindErrors(@"void MethodWithcycle(){ for (int i = 0; i<10;++i){} }");
			Assert.That(errors, Is.Null);
		}

		[Test]
		public void find_all_warnings()
		{
			var errors = FindErrors(@"
void A(){ if (true) return true; else return false; }
void B(){ if (false) return true; return false; }
");
			Assert.That(errors, Does.Contain("Строка 2"));
			Assert.That(errors, Does.Contain("Строка 3"));
		}

		private static string FindErrors(string code)
		{
			var errors = new ParametrNameLengthValidator().FindError(code);
			return errors;
		}
	}
}