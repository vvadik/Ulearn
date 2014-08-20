using System;
using uLearn;

namespace Slide01
{
	[Slide("Массивы", "{345ABA53-6896-4F19-92BE-3C102D09DEF6}")]
	public class S010_Array
	{
		//#video ukrsujsZtc0
		/*
		## Заметки по лекции
		*/

		public static void Main()
		{
			//Здесь мы объявляем переменную array, точно также, как раньше объявляли переменные других типов.
			//Тип массива чисел - это int[]. Аналогично, есть типы string[], double[], и т.д.
			int[] array;
			int number;

			//Здесь мы инициализируем переменную array значением "new int[3]" - новый массив длины 3. 
			array = new int[3];
			number = 10;

			//Обращение к элементам массива
			array[0] = 1;
			array[1] = 2;
			array[2] = 3;

			//Массив, как и остальные типы, имеет собственные свойства и методы
			Console.WriteLine(array.Length); //Выведет 3


			//Обратите внимание, что array.ToString() работает не так, как хотелось бы.
			Console.WriteLine(array.ToString()); //Выведет System.Int32[]

			//Этот код вызовет exception - выход за границы массива
			Console.WriteLine(array[100]);

		}

		public static void MainX()
		{
			var array = new[] { 1, 2, 3 };

			//Элементы массива можно пробежать так
			for (int i = 0; i < array.Length; i++)
				Console.WriteLine(array[i]);

			//Но лучше использовать специально предназначенный для этого оператор foreach
			foreach (var e in array)
				Console.WriteLine(e);

			//Иногда, например в случае присвоения элементов массива, foreach использовать не получается.
			for (int i = 0; i < array.Length; i++)
				array[i] = 2 * i;

			/* Общее правило по циклам такое:
             *  ИСПОЛЬЗУЙТЕ FOREACH, если это возможно
             *  ИСПОЛЬЗУЙТЕ FOR, если есть четко выделяемая переменная цикла, которая изменяется с каждой итерацией
             *  ИСПОЛЬЗУЙТЕ WHILE во всех остальных случаях
             */


		}
	}
}