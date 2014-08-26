using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U04_Arrays
{
	/*
	В этой задаче вам предстоит написать программу, которая определяет, есть ли победитель.

	Играть будем в [крестики-нолики](https://ru.wikipedia.org/wiki/%D0%9A%D1%80%D0%B5%D1%81%D1%82%D0%B8%D0%BA%D0%B8-%D0%BD%D0%BE%D0%BB%D0%B8%D0%BA%D0%B8)!
	
	Методу `GetGameResult` передается поле, представленное массивом 3х3 из `enum Markers`. 
	Вам надо вернуть победителя (`BlackPlayer`, `WhitePlayer`), если таковой имеется или `Draw`, 
	если выигрышной последовательности нет ни у одного, либо есть у обоих.

	Это задача заметно сложнее предыдущих. Постарайтесь придумать красивое, понятное решение.

	Подумайте, как разбить задачу на более простые подзадачи. Попытайтесь выделить один или два вспомогательных метода.
	
	*/
	[Slide("Крестики-нолики", "{B4F3138D-5CDB-4F8A-9976-E0F4D379687A}")]
	class S062_TicTacToe
	{
		public enum Mark
		{
			Empty, 
			Cross, 
			Circle
		}

		public enum GameResults 
		{ 
			CrossPlayer, 
			CirclePlayer, 
			Draw
		}

		[ExpectedOutput(@"
XXX
OO.
...
CrossPlayer

OXO
XO.
.XO
CirclePlayer

OXO
XOX
OX.
CirclePlayer

XOX
OXO
OXO
Draw

...
...
...
Draw

XXX
OOO
...
Draw


")]
		public static void Main()
		{
			Check("XXX OO. ...");
			Check("OXO XO. .XO");
			Check("OXO XOX OX.");
			Check("XOX OXO OXO");
			Check("... ... ...");
			Check("XXX OOO ...");
		}

		private static void Check(string description)
		{
			Console.WriteLine(description.Replace(" ", "\r\n"));
			Console.WriteLine(GetGameResult(CreateFromString(description)));
			Console.WriteLine();
		}

		[HideOnSlide]
		private static Mark[,] CreateFromString(string str)
		{
			var field = str.Split(' ');
			var ans = new Mark[3, 3];
			for (int x = 0; x < field.Length; x++)
				for (var y = 0; y < field.Length; y++)
					ans[x, y] = field[x][y] == 'X' ? Mark.Cross : (field[x][y] == 'O' ? Mark.Circle : Mark.Empty);
			return ans;
		}

		[Exercise]
		[Hint("На роль вспомогательного метода хорошо сойдет `bool HasWinSequence(Mark[,] field, Mark mark)`, определяющий есть ли выигрышная последовательность у mark. Он позволит не дублировать логику для крестиков и ноликов.")]
		[Hint("HasWinSequence может быть все ещё слишком сложным методом. Подумайте, как его можно разбить на подзадачи.")]
		[Hint(@"Каждую выигрышную линию можно задать четверкой (x0, y0, dx, dy) — координатой левой верхней ячейки этой линии (x0, y0) и направлением к остальным ячейкам этой линии (dx, dy).
Например, побочная диагональ задается четверкой (0, 2, 1, -1). 
Подумайте, как это можно применить!")]
		public static GameResults GetGameResult(Mark[,] field)
		{
			bool circleWin = HasWinSequence(field, Mark.Circle);
			bool crossWin = HasWinSequence(field, Mark.Cross);
			if (circleWin && crossWin) return GameResults.Draw;
			if (circleWin) return GameResults.CirclePlayer;
			if (crossWin) return GameResults.CrossPlayer;
			return GameResults.Draw;
		}

		[HideOnSlide]
		[ExcludeFromSolution]
		private static bool HasWinSequence(Mark[,] field, Mark mark)
		{
			return
				IsLine(field, mark, 0, 0, 1, 0)
				|| IsLine(field, mark, 0, 1, 1, 0)
				|| IsLine(field, mark, 0, 2, 1, 0)
				|| IsLine(field, mark, 0, 0, 0, 1)
				|| IsLine(field, mark, 1, 0, 0, 1)
				|| IsLine(field, mark, 2, 0, 0, 1)
				|| IsLine(field, mark, 0, 0, 1, 1)
				|| IsLine(field, mark, 2, 0, -1, 1);
		}

		[HideOnSlide]
		[ExcludeFromSolution]
		private static bool IsLine(Mark[,] field, Mark mark, int x0, int y0, int dx, int dy)
		{
			for(int i=0; i<3; i++)
				if (field[x0 + i*dx, y0 + i*dy] != mark) return false;
			return true;
		}
	}
}
