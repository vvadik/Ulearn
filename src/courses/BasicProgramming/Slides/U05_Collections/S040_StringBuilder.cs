using System;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;

namespace uLearn.Courses.BasicProgramming.Slides.U05_Collections
{
	//#video 5CzbNN73B3k
	/*
	## Заметки по лекции
	*/
	[Slide("StringBuilder", "{7D877273-0A8E-4257-8CA9-BDD90369DCDB}")]
    public class S040_StringBuilder
    {
		[Test]
        public static void Main()
        {
            //StringBuilder - это класс, представляющий собой изменяемую строку
            var builder = new StringBuilder();

            //Он содержит различные методы вставки, добавления, удаления и т.д.
            builder.Append("Some ");
            builder.Append("string ");
            builder.Append("#15");
            builder.Remove(0, 5);
            builder.Insert(0, "test ");

            //Также можно манипулировать отдельными символами
            builder[0] = 'T';

            //StringBuilder можно превратить в строку
            var str = builder.ToString();
            Console.WriteLine(str);

            //Не нужно полностью заменять строки на StringBuilder,
            //Только в тех случаях, когда действительно выполняется много преобразований
        }

		static void WrongConcatenation()
		{
			//Если нам нужно сконкатенировать большое количество строк
			//то конкатенация "в лоб" потребует очень много памяти в куче, 
			//и будет работать медленно

			var str = "";

			for (int i = 0; i < 50000; i++)
			{
				str += i.ToString() + ", ";
			}
		}

		static void RightConcatenation()
		{
			//Конкатенация со StringBuilder работает в сотни раз быстрее
			//Однако, в случае 3-4 строк вы не почувствуете разницы, поэтому 
			//в этом случае пользоваться StringBuilder не обязательно
			var builder = new StringBuilder();

			for (int i = 0; i < 50000; i++)
			{
				builder.Append(i);
				builder.Append(", ");
			}
		}

		[Test]
		[Explicit]
		public static void Main2()
		{
			var watch = new Stopwatch();
			watch.Start();
			WrongConcatenation();
			watch.Stop();
			Console.WriteLine(watch.ElapsedMilliseconds);

			watch = new Stopwatch();
			watch.Start();
			RightConcatenation();
			watch.Stop();
			Console.WriteLine(watch.ElapsedMilliseconds);
		}
    }
}