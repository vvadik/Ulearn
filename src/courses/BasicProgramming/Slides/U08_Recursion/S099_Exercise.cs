﻿namespace uLearn.Courses.BasicProgramming.Slides.U08_Recursion
{
	[Slide("Практика", "B8BE9F44E4FE433AA03AFC5CAB9195C5")]
	public class S099_Exercise
	{
		/*
		## Перестановки (2 балла)
		Напишите функцию `void MakePermutations(int[] permutation, int position, List<int[]> result)`, генерирующую все перестановки и сохраняющую результат в список result.

		Например, вызов `MakePermutations(new int[3], 0, result)` должен дать в списке result следующие перестановки:
		```
		012
		021
		102
		120
		201
		210
		```

		Продемонстрируйте работоспособность функции с помощью модульных тестов.


		## Крестики-нолики и перестановки (1 балл)

		Все клетки поля 3х3 для игры в крестики нолики можно пронумеровать числами от 0 до 8 слева направо, сверху вниз:
		```
		012
		345
		678
		```

		Тогда любую перестановку размера 9 можно интерпретировать как последовательность ходов в некоторой партии в крестики нолики (возможно, включая несколько ходов после победы одного из игроков).
		
		Напишите функцию `char[,] PermutationToBoard(int[] permutation)`, переводящую перестановку в финальное состояние игрового поля — двумерный массив 3х3.

		Напишите функцию `string BoardToString(char[,] board)`, переводящую состояние игрового поля в строковое представление следующего вида:
		```
		XOX
		OXO
		OXX
		```

		Скомбинируйте все три написанные функции и выведите на консоль все различные финалы, возможные в игре, которая началась с ходов 0,4,2,1,7,6.

		## Крестики против ноликов (1 балл)

		Партия — это последовательность ходов, заканчивающаяся как только либо кто-то выиграл, либо когда все поле заполнено.

		Используйте написанные ранее функции для вычисления следующих статистик об игре в крестики нолики 3х3:

		* количество различных партий (симметричные партии считаются различными);
		* количество партий, в которых выигрывают крестики (игрок, делающий ход первым);
		* количество партий, в которых выигрывают нолики;
		* количество ничейных партий.
		*/
	}
}