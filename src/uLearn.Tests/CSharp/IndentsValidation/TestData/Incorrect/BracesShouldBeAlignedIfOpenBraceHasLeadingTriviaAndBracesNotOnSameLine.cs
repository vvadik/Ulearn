using System.Collections.Generic;

namespace uLearn.CSharp.IndentsValidation.TestData.Incorrect
{
	public class BracesShouldBeAlignedIfOpenBraceHasLeadingTriviaAndBracesNotOnSameLine
	{
		public static void M6(string[] args)
		{
			var a = 0;
	}

		public static void M(string[] args)
		{
			var a = 0; }

		public static void M1(string[] args)
		{
			var a = 0;      }

		public static void M4(string[] args)
		{
			var b = 0;
			var a = 0;     }

		public static void M5(string[] args)
		{
			var b = 0;
			var a = new List<int> { 1, 2, 3 };     }
		

		public static void Main1(string[] args)
		{
			var c = 0;
			}

		public static void Main7(string[] args)
		{
			var c = 0;
}

		public static void Main3(string[] args)
		{
			var c = 0;
					}

		public static void Main2(string[] args)
		{
			var d = new[] 
				{
					1, 2, 3
			};

			var d1 = new[] 
			{
				1, 2, 3
				};

			var d2 = new[] 
			{
				1, 2, 3
		};

			var e = new[] 
				{
					new[]
					{
						1
				},
					new[] 
					{
						2
				}
			};

			for (int i = 0; i < 10; i++)
			{
				var a = 0
			; }

			for (int i = 0; i < 10; i++)
			{
				var
			a = 0; }
		}
	}
}