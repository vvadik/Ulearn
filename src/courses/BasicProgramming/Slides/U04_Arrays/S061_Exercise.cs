using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uLearn.CSharp;

namespace uLearn.Courses.BasicProgramming.Slides.U04_Arrays
{
	/*
	В этой задаче вам предстоит написать программу, которая определяет, есть ли победитель.
	Играть будем в [крестики-нолики](https://ru.wikipedia.org/wiki/%D0%9A%D1%80%D0%B5%D1%81%D1%82%D0%B8%D0%BA%D0%B8-%D0%BD%D0%BE%D0%BB%D0%B8%D0%BA%D0%B8)!
	Методу ```GetGameResult``` передается поле, представленное массивом 3х3 из ```enum Markers```. Вам надо вернуть 
	победителя(```BlackPlayer```, ```WhitePlayer```), если таковой имеется. ```Draw```, если ничья.
	Результаты из ```enum GameResults```.
	*/
	[Slide("Крестики-нолики", "{B4F3138D-5CDB-4F8A-9976-E0F4D379687A}")]
	class S061_Exercise
	{
		public enum Marks { Empty, Cross, Circle }
		public enum GameResults { CrossPlayer, CirclePlayer, Draw }

		[ExpectedOutput("CrossPlayer\nCirclePlayer\nCirclePlayer\nDraw")]
		public static void Main()
		{
			var results = new GameResults[]
			{
				GetGameResult(CreateFromString("XXX OOE EEE")),
				GetGameResult(CreateFromString("OXO XOE EXO")),
				GetGameResult(CreateFromString("OXO XOX OXE")),
				GetGameResult(CreateFromString("XOX OXO OXO"))
			};

			foreach (var res in results)
				Console.WriteLine(res);
		}

		[HideOnSlide]
		private static Marks[,] CreateFromString(string str)
		{
			var field = str.Split(' ');
			var ans = new Marks[3, 3];
			for (int x = 0; x < field.Length; x++)
				for (var y = 0; y < field.Length; y++)
					ans[x, y] = field[x][y] == 'X' ? Marks.Cross : (field[x][y] == 'O' ? Marks.Circle : Marks.Empty);
			return ans;
		}

		[Exercise]
		[Hint("Просто запишите правила крестиков-ноликов на языке С#")]
		public static GameResults GetGameResult(Marks[,] field)
		{
			var winPositions = new string[][]
			{
				new[] {"00", "10", "20"}, new[] {"00", "01", "02"}, new[] {"00", "11", "22"}, 
				new[] {"01", "11", "21"}, new[] {"02", "12", "22"}, new[] {"10", "11", "12"}, 
				new[] {"20", "21", "22"}, new[] {"02", "11", "20"}
			};

			foreach (var e in winPositions)
			{
				var currentCellMark = field[e[0].First() - '0', e[0].Last() - '0'];
				var count = 0;
				foreach (var cell in e)
				{
					var x = cell.First() - '0';
					var y = cell.Last() - '0';
					if (currentCellMark == field[x, y])
						count++;
					else
						break;
				}
				if (count == 3)
					return GetWinner(currentCellMark);
			}
			return GameResults.Draw;
			/*uncomment
			...
			*/
		}

		[HideOnSlide]
		[ExcludeFromSolution]
		public static GameResults GetWinner(Marks type)
		{
			return type == Marks.Circle ? GameResults.CirclePlayer : GameResults.CrossPlayer;
		}
	}
}
