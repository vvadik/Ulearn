using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Title("While")]
	class S06_While
	{
		/*
		##Задача: While
		
		Иногда заказчикам требуются невероятные вещи. Вот, например, сейчас, клиенту нужна программа,
		которая считает сколько раз надо умножить одно положительное число на другое положительное число,
		чтобы оно переполнилось из сандартного типа, который, в добавок, может быть нескольких видов.
		К счастью, гарантировали, что вам не попадутся еденички в качестве второго числа.
		*/

		public enum Types
		{
			Integer, //int
			LongInteger //long
		}

		
		[Hint(@"Обычно, когда переменная подобная переменной типа int переполняется, она становится отрицтельной.
		      Это обусловлено способом хранения числа в двоичном представлении.
		      Его даже можно переполнить снова, и оно опять станет положительным.")]
		[ExpectedOutput("13\r\n7\r\n63\r\n9")]
		[ShowOnSlide]
		public static void Main()
		{
			Console.WriteLine(OverflowPowerNumber(Types.Integer, "3", 5));
			Console.WriteLine(OverflowPowerNumber(Types.Integer, "13", 15));
			Console.WriteLine(OverflowPowerNumber(Types.LongInteger, "1", 2));
			Console.WriteLine(OverflowPowerNumber(Types.LongInteger, "90", 100));
		}

		[Exercise]
		private static int OverflowPowerNumber(Types type, string accumulatorNumber, int multiplier)
		{
			return type == Types.Integer ? CalcForInteger(int.Parse(accumulatorNumber), multiplier) : CalcForLong(long.Parse(accumulatorNumber), multiplier);
			/*uncomment
			...
			*/
		}

		private static int CalcForLong(long accumulator, int multiplier)
		{
			int count = 0;
			while (accumulator > 0)
			{
				count++;
				accumulator *= multiplier;
			}
			return count;
		}

		private static int CalcForInteger(int accumulator, int multiplier)
		{
			int count = 0;
			while (accumulator > 0)
			{
				count++;
				accumulator *= multiplier;
			}
			return count;
		}
	}
}
