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
	public class VerbInMethodNameValidator_should
	{
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
		[TestCase(@"public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); //финализатор не будет вызываться
        }")]
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
			new VerbInMethodNameValidator().FindError(code);
	}
}