using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uLearn.Courses.BasicProgramming.Slides
{
	class S08_Continue
	{
		/*
		##Задача: Continue - На лугу, на лугу, на лугу пасутся ко...
		Представьте, что вы коровка, и пасетесь на лугу. Несомненно, вы будете кушать только самую вкусную травку, а когда наедитесь, то пойдете домой.
		Напишите себе алгоритм, как вы будете кушать травку :) Каждая травинка имеет свой порядковый номер. Необходимо пользоваться такими правилами: 
		 * Перебрать все числа от 0 до переданного значения maxCount включительно - это порядковые номера травинок.
		 * Если фильтр Filter от значения number выдает true, значит травка с этим номером невкусная, и её есть не надо.
		 * Если показатель сытости Satiety >= 100, следовательно вы уже наелись и надо уйти домой.
		 * Чтобы съесть травку - нужно использовать функцию EatGrass от number, где number - номер текущей травки
		 * Если необходимо уйти домой, или травка кончилась (достигли значения maxCount), просто выведите текущее значение Satiety и выйдите из функции
		*/

		public static int Satiety = 0;

		
		[ShowOnSlide]
		[ExpectedOutput("15\r\n100\r\n57")]
		public static void Main()
		{
			Eating(10);
			Satiety = 0;
			Eating(25);
			Satiety = 0;
			Eating(19);
			Satiety = 0;
		}

		[Exercise]
		public static void Eating(int maxCount)
		{
			int number = 0;
			while (number < maxCount)
			{
				if (Filter(number))
				{
					number++;
					continue;
				}
				if (Satiety >= 100)
					break;
				EatGrass(number);
				number++;
			}
			Console.WriteLine(Satiety);
			/*uncoment
			...
			*/
		}

		[ShowOnSlide]
		private static void EatGrass(int number)
		{
			Satiety += Math.Abs(number.GetHashCode())%30;
		}

		[ShowOnSlide]
		private static bool Filter(int number)
		{
			return (number*number)%3 == number%3;
		}
	}
}
