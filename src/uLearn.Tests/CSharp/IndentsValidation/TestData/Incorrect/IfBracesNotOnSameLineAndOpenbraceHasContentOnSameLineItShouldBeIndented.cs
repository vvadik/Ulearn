using System.Collections.Generic;

namespace uLearn.CSharp.IndentsValidation.TestData.Incorrect
{
	public class IfBracesNotOnSameLineAndOpenbraceHasContentOnSameLineItShouldBeIndented
	{ // если после фигурной скобки на той же строке что-то есть, это что-то должно быть перенесено на новую строку с отступом
		public static void Main17(string[] args) { var a = 0;
			var b = 0; var c = 0;
		}

		public static void Main3(string[] args)
		{    var a = 0; var b = 0; var c = 0;
		}

		public static void Main10(string[] args)
		{var a = 0; var b = 0; var c = 0;
		}

		public static void Main7(string[] args)
		{var a = 0;
			var b = 0;
			var c = 0;
		}

		Dictionary<int, int> dl = new Dictionary<int, int>
		{
			{ 1, 2
			}
		};

		Dictionary<int, int> dl1 = new Dictionary<int, int>
		{
			{ 1,
				2
			}
		};
	}
}