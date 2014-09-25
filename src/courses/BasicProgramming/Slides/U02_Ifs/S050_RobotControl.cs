using System;
using System.Linq;
using NUnit.Framework;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U02_Ifs
{
	[Slide("Управление роботом", "{50AC4FC7-A6E1-4C4E-8C61-ECC75AEB912E}")]
	class S050_RobotControl : SlideTestBase
	{
		/*
		В воскресенье Вася пошел в кружок робототехники и увидел там такой код управления боевым роботом:
		*/

		public delegate bool ShouldFireDelegate(bool enemyInFront, string enemyName, int enemyHealth, int robotFirePower, int robotHealth);

		[ExcludeFromSolution]
		static bool ShouldFire(
			bool enemyInFront, string enemyName, int enemyHealth,
			int robotFirePower, int robotHealth)
		{
			bool shouldFire = false;
			if (enemyInFront == true)
			{
				if (enemyName == "Big boss")
				{
					if (robotHealth < 50) shouldFire = false;
					if (robotHealth > 100) shouldFire = true;
				}
				if (robotFirePower > enemyHealth) return true;
				if (enemyHealth < robotHealth) shouldFire = true;
			}
			else
			{
				return false;
			}
			return shouldFire;
		}

		/*
		Код показался Васе слишком длинным, а к тому же еще и неряшливым.
		Вася поспорил с автором, что сможет написать функцию,
		делающую ровно то же самое, но всего в один оператор.

		Кажется, Вася погорячился... Или нет? Помогите ему не проиграть в споре!
		*/

		[Test]
		[ExcludeFromSolution]
		[HideOnSlide]
		public void Test()
		{
			Check(ShouldFire, true);
		}

		[ExpectedOutput(@"Functions are the same!")]
		[HideOnSlide]
		public static void Main()
		{
			Check(ShouldFire2, false);
		}

		[HideOnSlide]
		private static void Check(ShouldFireDelegate shouldFireFunction, bool throwOnError)
		{
			var errors =
				from enemyInFront in new[] {true, false}
				from enemyName in new[] {"Big boss", "Zombie", null}
				from enemyHealth in new[] {1000, 0, 10, 1}
				from robotFirePower in new[] {10, 100, 1}
				from robotHealth in new[] {10, 65, 110, 2, 20}
				let shouldFire = enemyInFront &&
				   (enemyName == "Big boss" && robotHealth > 100
				   || robotFirePower > enemyHealth || robotHealth > enemyHealth)
				where shouldFire != shouldFireFunction(enemyInFront, enemyName, enemyHealth, robotFirePower, robotHealth)
				select
					string.Format("Functions are different on the input ({0}, \"{1}\", {2}, {3}, {4})",
						enemyInFront, enemyName, enemyHealth, robotFirePower, robotHealth);
			foreach (var error in errors)
			{
				if (throwOnError) throw new Exception(error);
				Console.WriteLine(error);
				return;
			}
			Console.WriteLine("Functions are the same!");
		}

		[Exercise]
		[Hint("Грамотно используйте конъюнкцию вместе с дизъюнкцией")]
		[SingleStatementMethod]
		static bool ShouldFire2(
			bool enemyInFront, string enemyName, int enemyHealth,
			int robotFirePower, int robotHealth)
		{
			return enemyInFront &&
					(enemyName == "Big boss" && robotHealth > 100
					|| robotFirePower > enemyHealth 
					|| robotHealth > enemyHealth);
			/*uncomment
			return ...
			*/
		}

	}
}
