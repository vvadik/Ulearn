using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Continue", "{BE2A0C11-20F5-4351-96F7-07D60FFE73CF}")]
	class S08_Continue
	{
		/*
		##Задача: Continue - На лугу, на лугу, на лугу пасутся ко...
		
		Вы попали на ферму, и вас просят написать код для распределительной машины. Она распределяет коров по стойлам. У каждой коровки есть свой собственный порядковый номер, 
		и ходят коровки строем, соответственно их номерам. Все они поочереди проходят через распределительный автомат, и каких-то коров отправляют в новые стойла, а каких-то в старые.
		Вам необходимо реализовать этот алгоритм, вот что необходимо сделать:
		
		 * Перебрать все числа от 0 до переданного значения ```maxCount``` включительно - это порядковые номера коров.
		 * Если фильтр ```Filter``` от значения number выдает true, значит коровка под номером ```number``` достойна пойти в новое стойло.
		 * Если показатель наполненности нового стойла ```Fullness``` >= 100, то все остальные коровки идут в старое стойло.
		 * Чтобы поместить корову в новое стойло, необходимо вызвать функцию ```PutCow```. Если в старое - ничего не делать, коровка сама найдет дорогу. ```PutCow``` автоматически увеличит показатель наполненности.
		 * Если кончились коровы, или кончились места, просто выведите текущее значение ```Fullness``` и выйдите из функции.
		*/

		public static int Fullness = 0;

		
		[ExpectedOutput("42\r\n112\r\n68")]
		public static void Main()
		{
			Distribute(10);
			Fullness = 0;
			Distribute(25);
			Fullness = 0;
			Distribute(13);
			Fullness = 0;
		}

		[Exercise]
		public static void Distribute(int maxCount)
		{
			int number = 0;
			while (number < maxCount)
			{
				if (Filter(number))
				{
					number++;
					continue;
				}
				if (Fullness >= 100)
					break;
				PutCow(number);
				number++;
			}
			Console.WriteLine(Fullness);
			/*uncomment
			...
			*/
		}

		private static void PutCow(int number)
		{
			Fullness += ((number*397*13)%199)%30;
		}

		private static bool Filter(int number)
		{
			return (number*number)%3 == number%3;
		}
	}
}
