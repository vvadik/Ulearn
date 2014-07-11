using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Title("Оператор условия")]
	class S03_IfOperator
	{
		/*
		##Задача: Оператор if.
		Вася купил компьютер и теперь выбирает себе имя пользователя.
		У него есть список всех имен мира, и теперь он хочет для каждого имя посчитать его "значение".
		И уже отталкиваясь от этих значений, он выберет всего имя. Каждому имени нужно сопоставить число, вычисляемое таким образом:
		 * Изначальная стоимость имени = 0
		 * Если в слове есть буква "z", то стоимость увеличивается на 10
		 * Если в слове есть буквосочетание "ar", то стоимость увеличивается на 8, иначе уменьшается на 6
		 * Если в слове есть буква "q", то:
			 * если нет буква "w", то стоимость увеличивается на 100
			 * если нет буква "e", то стоимость увеличивается на 150
			 * если есть буква "x", то стоимость увеличивается на 90
		   иначе, уменьшается на 20
		*/
		[ShowOnSlide]
		[ExpectedOutput("8\r\n84\r\n4\r\n244")]
		public static void Main()
		{
			int firstNameValue = GetValue("ararat");
			int secondNameValue = GetValue("qwertyx");
			int thirdNameValue = GetValue("zweqra");
			int fourthNameValue = GetValue("qqNAMEqq");
			Console.WriteLine(firstNameValue);
			Console.WriteLine(secondNameValue);
			Console.WriteLine(thirdNameValue);
			Console.WriteLine(fourthNameValue);
		}

		[Exercise]
		private static int GetValue(string name)
		{
			int value = 0;
			if (name.Contains("ar")) value += 8; else value -= 6;
			if (name.Contains("z")) value += 10;
			if (name.Contains("q"))
			{
				if (!name.Contains("w")) value += 100;
				if (!name.Contains("e")) value += 150;
				if (name.Contains("x")) value += 90;
			}
			return value;
			/*uncomment
			return 0;
			*/
		}
	}
}
