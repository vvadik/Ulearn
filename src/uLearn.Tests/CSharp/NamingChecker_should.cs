using NUnit.Framework;

namespace uLearn.CSharp
{
	[TestFixture]
	public class NamingChecker_should
	{
		[TestCase("publid void GetSomething() {}")]
		[TestCase("static void GetSomething(int a) {}")]
		[TestCase("virtual void Get(int a) {}")]
		[TestCase("override void Get() {}")]
		[TestCase("public static void get(out int a, dynamic x) {}")]
		[TestCase("protected override void getX(ref int x) {}")]
		public void Warn_VoidGetMethods(string code)
		{
			CheckIncorrect(code, "Get");
		}

		[TestCase("protected override void setX() {}")]
		[TestCase("void set() {}")]
		[TestCase("public static dynamic Set() {}")]
		[TestCase("public static SomeResult SetSomething() {}")]
		public void Warn_NoArgsSetMethods(string code)
		{
			CheckIncorrect(code, "Set");
		}

		[TestCase("public static void Moving() {}")]
		[TestCase("Treasure Searching(int x) { return null; }")]
		[TestCase("void initializing() { }")]
		public void Warn_IngMethods(string code)
		{
			CheckIncorrect(code, "ing");
		}

		[TestCase("public static void StopMoving() {}")]
		[TestCase("Treasure skip_searching(int x) { return null; }")]
		[TestCase("void StopProcessIfItIsLongRunning() { }")]
		public void Pass_ComplexIngNames(string code)
		{
			CheckCorrect(code);
		}

		[TestCase("int GetX(){return 42;}")]
		[TestCase("public static string Get(){return string.Empty;}")]
		[TestCase("override object get_X(){return null;}")]
		public void Pass_NonVoidGetMethods(string code)
		{
			CheckCorrect(code);
		}

		[TestCase("publid void DoSomething() {}")]
		[TestCase("static void SetSomething(int a) {}")]
		[TestCase("virtual void Pet(int a) {}")]
		[TestCase("override void Met() {}")]
		[TestCase("public static void RunNuget() {}")]
		[TestCase("protected override void NotGetX(ref int x) {}")]
		public void Pass_NonGetMethods(string code)
		{
			Assert.That(FindErrors(code), Is.Null, code);
		}

		private static void CheckCorrect(string code)
		{
			Assert.That(FindErrors(code), Is.Null, code);
		}

		private void CheckIncorrect(string incorrectCode, string messageSubstring)
		{
			Assert.That(FindErrors(incorrectCode), Does.Contain("Строка 1").And.Contain(messageSubstring), incorrectCode);
		}

		private static string FindErrors(string code)
		{
			return new NamingStyleValidator().FindError(code);
		}
	}
}