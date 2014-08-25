using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Управление роботом", "{50AC4FC7-A6E1-4C4E-8C61-ECC75AEB912E}")]
	class S032_RobotControl
	{
		/*
		В воскресенье Вася пошел в кружок робототехники и увидел там такой код управления боевым роботом:
		*/

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
				else
				{
					if (robotFirePower > enemyHealth) return true;
					else if (enemyHealth < robotHealth) shouldFire = true;
					else shouldFire = false;
				}
			}
			else
			{
				return false;
			}
			return shouldFire;
		}

		/*
		Код показался Васе слишком длинным и он поспорил с автором, что сможет написать функцию,
		делающую ровно то же самое, но всего в один оператор.

		Кажется, Вася погорячился... Или нет? Помогите ему не проиграть в споре!
		*/

		[ExpectedOutput(@"
False ?= False
False ?= False
True ?= True
True ?= True
True ?= True
True ?= True
True ?= True
False ?= False
False ?= False
False ?= False
")]
		[Hint("Грамотно используйте конъюнкцию вместе с дизъюнкцией")]
		public static void Main()
		{
			CompareFunctions(true, "Big boss", 1000, 10, 10);
			CompareFunctions(true, "Big boss", 1000, 10, 65);
			CompareFunctions(true, "Big boss", 1000, 10, 110);
			CompareFunctions(true, "Big boss", 0, 10, 110);
			CompareFunctions(true, "Zombie", 10, 100, 2);
			CompareFunctions(true, "Zombie", 10, 100, 20);
			CompareFunctions(true, "Zombie", 10, 1, 20);
			CompareFunctions(true, "Zombie", 10, 10, 10);
			CompareFunctions(false, "Zombie", 1, 100, 100);
			CompareFunctions(false, null, 0, 100, 100);
		}

		private static void CompareFunctions(
			bool enemyInFront, string enemyName, int enemyHealth, 
			int robotFirePower, int robotHealth)
		{
			Console.WriteLine("{0} ?= {1}", 
				ShouldFire(enemyInFront, enemyName, enemyHealth, robotFirePower, robotHealth),
				ShouldFire2(enemyInFront, enemyName, enemyHealth, robotFirePower, robotHealth));
		}


		[Exercise]
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
