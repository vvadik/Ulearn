using NUnit.Framework;

namespace uLearn.CSharp
{
	[TestFixture]
	public class NamingLengthValidator_should
	{
		[TestCase(@"public class SomeClass{public readonly int N {get;}")]
		[TestCase(@"public class SomeClass{public readonly int N => 0;")]
		[TestCase(@"public class SomeClass{private int A { get; set; }}")]
		[TestCase(@"public class SomeClass{public int N { get; set; }}")]
		[TestCase(@"public class SomeClass{public int X1 { get; set; }}")]
		[TestCase(@"public class SomeClass{public int N12 { get; set; }}")]
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
		[TestCase(@"public class SomeClass{int a;}")]
		[TestCase(@"public class SomeClass{int a => 0;}")]
		[TestCase(@"public class SomeClass{int a12;}")]
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
		[TestCase(@"public static Rational operator *(Rational r1){}")]
		[TestCase(@"public static Rational operator *(Rational r12){}")]
		[TestCase(@"public static Rational operator *(Rational r123){}")]
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
		[TestCase("bool SomeMethod(int x, int y, int z){ var s = \"string\"; return b; }")]
		[TestCase(@"void SomeMethod(int numberOfErrors){ int a; }")]
		[TestCase(@"void SomeMethod(int numberOfErrors){ int a1; }")]
		[TestCase(@"void SomeMethod(){ var p1 = new Point(); }")]
		[TestCase(@"void SomeMethod(int numberOfErrors){ int a12; }")]
		[TestCase(@"void SomeMethod(int numberOfErrors){ var b = new byte[1]; }")]
		public void warn_local_variables_short_name(string code)
		{
			var errors = FindErrors(code);
			Assert.That(errors, Is.Not.Null);
		}

		[TestCase(@"void SomeMethod(){ string someStr; }")]
		[TestCase(@"void SomeMethod(){ Point p; }")]
		[TestCase(@"void SomeMethod(){ var p = new Point(); }")]
		public void ignore_correct_local_variables_names(string code)
		{
			var errors = FindErrors(code);
			Assert.That(errors, Is.Null);
		}

		[TestCase(@"void SomeMethod(){ for (var n=0;n<10;++n){} }")]
		[TestCase(@"void SomeMethod(){ for (char n=0;n<10;++n){} }")]
		[TestCase(@"void SomeMethod(){ for (var i=0;i<10;++i){var a = 0;} }")]
		[TestCase(@"void SomeMethod(){ for (var i=0;i<10;++i){var a12 = 0;} }")]
		public void warn_cycles_with_short_name(string code)
		{
			var errors = FindErrors(code);
			Assert.That(errors, Is.Not.Null);
		}

		[TestCase(@"void SomeMethod(){ for (var i=0;i<10;++i) for (var j=0;j<10;++j){} }")]
		[TestCase(@"void SomeMethod(){ for (char j=0;j<10;++j){var p = new Point();} }")]
		[TestCase(@"void SomeMethod(){ for (char l=0;l<10;++l){int x; int y;} }")]
		[TestCase(@"void SomeMethod(){ for (char k=0;k<10;++k){} }")]
		[TestCase(@"void SomeMethod(){ for (char x=0;x<10;++x){} }")]
		public void ignore_cycles_with_correct_short_name(string code)
		{
			var errors = FindErrors(code);
			Assert.That(errors, Is.Null);
		}
		
		[TestCase(@"void SomeMethod(string[] array){ array.Select(x => x[0]).Where(a => a); }")]
		[TestCase(@"void SomeMethod(string[] array){ array.Select(b => b[0]); }")]
		[TestCase(@"void SomeMethod(string[] array){ array.Select(C => C[0]); }")]
		[TestCase(@"void SomeMethod(string[] array){ array.Select(b1 => b1[0]); }")]
		[TestCase(@"char[] SomeMethod(string[] array){ return array.Select(b1 => b1[0]); }")]
		public void warn_expressions_with_short_name(string code)
		{
			var errors = FindErrors(code);
			Assert.That(errors, Is.Not.Null);
		}

		
		[TestCase(@"void SomeMethod(string[] array){ array.Select(x => x[0]) }")]
		[TestCase(@"void SomeMethod(string[] array){ array.Select(y => y[0]) }")]
		public void ignore_expressions_with_correct_short_name(string code)
		{
			var errors = FindErrors(code);
			Assert.That(errors, Is.Null);
		}
		
		private static string FindErrors(string code)
		{
			var errors = new NamimgLengthValidator().FindError(code);
			return errors;
		}
	}
}