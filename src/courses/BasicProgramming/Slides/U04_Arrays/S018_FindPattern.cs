using System;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U04_Arrays
{
	[Slide("Поиск массива в массиве", "{94B9A357-8F8F-4E6A-9497-C812A751F091}")]
	class S018_FindPattern : SlideTestBase
	{
		/*
		Вы с Васей решили, что одиночные циклы освоили в достаточной мере. Пора браться за что-то чуть более сложное!

		Что если в массиве нужно найти не один элемент, а целый подмассив?

		Если подмассив найден в массиве, то вернуть нужно минимальный индекс, с которого начинается подмассив в исходном массиве.

		Более строго это можно записать, если обозначить массив array, а подмассив subarray: 
		функция должна вернуть такое минимальное k, что array[k+i] == subarray[i] для всех i от 0 до subarray.Length-1.

		Если подмассив не найден, то вернуть нужно -1.

		Считайте, что пустой подмассив содержится в любом массиве, начиная с индекса 0.

		### Примечание
		
		Задача поиска подмассива в массиве эквивалентна задаче поиска подстроки в строке, для которой существует целое множество эффективных алгоритмов.
		В этой задаче достаточно реализовать самый простой и тривиальный. 
		Однако, если вам интересно, как искать подстроку за линейное время от длины строки, можете начать изучение этой области с обзорной статьи "[Поиск подстроки](https://ru.wikipedia.org/wiki/%D0%9F%D0%BE%D0%B8%D1%81%D0%BA_%D0%BF%D0%BE%D0%B4%D1%81%D1%82%D1%80%D0%BE%D0%BA%D0%B8)" в википедии.

		*/

		[ExpectedOutput(@"
0
1
1
0
0
-1
-1
-1
0
2
0
")]
		public static void Main()
		{
			Console.WriteLine(FindSubarrayStartIndex(new[] { 100, 100, 100, 100 }, new[] { 100, 100 }));
			Console.WriteLine(FindSubarrayStartIndex(new[] { 1, 2, 3 }, new[] { 2 }));
			Console.WriteLine(FindSubarrayStartIndex(new[] { 1, 2, 3 }, new[] { 2, 3 }));
			Console.WriteLine(FindSubarrayStartIndex(new[] { 1, 2, 3 }, new[] { 1, 2 }));
			Console.WriteLine(FindSubarrayStartIndex(new[] { 1, 2, 3 }, new[] { 1, 2, 3 }));
			Console.WriteLine(FindSubarrayStartIndex(new[] { 1, 2, 3 }, new[] { 1, 3 }));
			Console.WriteLine(FindSubarrayStartIndex(new[] { 1 }, new[] { 1, 2, 3 }));
			Console.WriteLine(FindSubarrayStartIndex(new int[0], new[] { 1, 2, 3 }));
			Console.WriteLine(FindSubarrayStartIndex(new[] { 1, 2, 3 }, new int[0]));
			Console.WriteLine(FindSubarrayStartIndex(new[] { 1, 2, 1, 2, 3 }, new[] {1, 2, 3}));
			Console.WriteLine(FindSubarrayStartIndex(new int[0], new int[0]));
		}

		[Exercise]
		[CommentAfterExerciseIsSolved("Существует целое семейство алгоритмов поиска подстроки в строке, использующие различные оптимизации.")]
		[Hint("Одним циклом тут не обойтись")]
		[Hint("Напишите сначала вспомогательный метод, который проверяет, входит ли подмассив в массив начиная с заданного индекса")]
		public static int FindSubarrayStartIndex(int[] array, int[] subArray)
		{
			for (int i = 0; i < array.Length - subArray.Length + 1; i++)
				if (ContainsAtIndex(array, subArray, i)) return i;
			return -1;
		}
		
		[ExcludeFromSolution]
		[HideOnSlide]
		private static bool ContainsAtIndex(int[] array, int[] subArray, int startIndex)
		{
			for (int i = 0; i < subArray.Length; i++)
				if (startIndex + i >= array.Length || array[startIndex + i] != subArray[i])
					return false;
			return true;
		}
	}
}
