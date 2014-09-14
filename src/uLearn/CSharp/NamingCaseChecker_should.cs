using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace uLearn.CSharp
{
	[TestFixture]
	public class NamingCaseChecker_should
	{
		[Test]
		public void inspect_className()
		{
			CheckCorrect("public class A{}");
			CheckIncorrect("internal class a{}");
		}
		
		[Test]
		public void inspect_structName()
		{
			CheckCorrect("public struct A{}");
			CheckIncorrect("internal struct a{}");
		}
		
		[Test]
		public void inspect_enumName()
		{
			CheckCorrect("public enum A{}");
			CheckIncorrect("internal enum a{}");
		}

		[Test]
		public void inspect_enumMemberName()
		{
			CheckCorrect("enum A{ B, C}");
			CheckIncorrect("enum A { B, c}");
		}

		[Test]
		public void inspect_typeParameter()
		{
			CheckCorrect("public class A<T>{}");
			CheckIncorrect("class a<T>{}");
		}

		[Test]
		public void inspect_methodName()
		{
			CheckCorrect("class A{ public void A(){} }");
			CheckCorrect("class A{ void A(){} }");
			CheckCorrect("class A{ void a(){} }");
			CheckCorrect("class A{ private void a(){} }");
			CheckCorrect("class A{ private void A(){} }");
			CheckCorrect("class A{ internal void a(){} }");
			CheckCorrect("class A{ internal void A(){} }");
			CheckIncorrect("class A{ public void a(){} }");
		}

		[Test]
		public void inspect_fieldName()
		{
			CheckCorrect("class A{ int A; }");
			CheckCorrect("class A{ int a; }");
			CheckCorrect("class A{ public int A; }");
			CheckIncorrect("class A{ public int a; }");
			CheckCorrect("class A{ private int a; }");
			CheckIncorrect("class A{ private int A; }");
			CheckCorrect("class A{ internal int a; }");
			CheckCorrect("class A{ internal int A; }");
			CheckCorrect("class A{ public int A, B, C; }");
			CheckIncorrect("class A{ public int A, B, c; }");
			CheckIncorrect("class A{ private int a, b, C; }");
		}

		[Test]
		public void inspect_varName()
		{
			CheckCorrect("class C{ int M(){int a;} }");
			CheckCorrect("class C{ int M(){int a, b, c;} }");
			CheckIncorrect("class C{ int M(){int A;} }");
			CheckIncorrect("class C{ int M(){int a, B;} }");
			CheckIncorrect("class C{ int M(){ Action a => { int a, B;} } }");
			CheckCorrect("class C{ int M(){ Action a => { int a, b;} } }");
		}

		private void CheckCorrect(string correctCode)
		{
			Assert.That(FindErrors(correctCode), Is.Null, correctCode);
		}

		private void CheckIncorrect(string incorrectCode)
		{
			Assert.That(FindErrors(incorrectCode), Is.StringContaining("Строка 1"), incorrectCode);
		}

		private static string FindErrors(string code)
		{
			var tree = CSharpSyntaxTree.ParseText(code);
			return new NamingCaseStyleValidator().FindError(tree);
		}
	}
}