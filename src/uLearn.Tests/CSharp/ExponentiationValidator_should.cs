using NUnit.Framework;

namespace uLearn.CSharp
{
	[TestFixture]
	public class ExponentiationValidator_should
	{
		[Test]
		public void ignore_when_no_math_pow()
		{
			var errors = FindErrors("int A() { return 2; }");
			Assert.That(errors, Is.Null);
		}
		
		[Test]
		public void ignore_when_degree_greater_than_three()
		{
			var errors = FindErrors("void A() { var b = Math.Pow(2, 5); }");
			Assert.That(errors, Is.Null);
		}
		
		[Test]
		public void warn_when_degree_equals_two()
		{
			var errors = FindErrors("void A() { var b = Math.Pow(2, 2); }");
			Assert.That(errors, Is.Not.Null);
		}
		
		[Test]
		public void warn_when_degree_equals_three()
		{
			var errors = FindErrors("void A() { var b = Math.Pow(2, 3); }");
			Assert.That(errors, Is.Not.Null);
		}
		
		[Test]
		public void analyse_code_with_semantic_model_if_possible()
		{
			var errors = FindErrors(@"
using System;

public class C 
{
	public int A() 
	{ 
		return Math.Pow(2, 3);
	}
}");
			Assert.That(errors, Is.Not.Null);
		}

		[Test]
		public void find_all_warnings()
		{
			var errors = FindErrors(@"
void A(){ var b = Math.Pow(2, 3); }
void B(){ var b = Math.Pow(2, 2); }
");
			Assert.That(errors, Does.Contain("Строка 2"));
			Assert.That(errors, Does.Contain("Строка 3"));
		}

		private static string FindErrors(string code)
		{
			var errors = new ExponentiationValidator().FindError(code);
			return errors;
		}
	}
}