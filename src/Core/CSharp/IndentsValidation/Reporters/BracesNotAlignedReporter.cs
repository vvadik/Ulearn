using System.Collections.Generic;
using System.Linq;

namespace uLearn.CSharp.IndentsValidation.Reporters
{
	internal static class BracesNotAlignedReporter
	{
		public static IEnumerable<string> Report(BracesPair[] bracesPairs)
		{
			foreach (var braces in bracesPairs.Where(pair => pair.Open.GetLine() != pair.Close.GetLine()))
			{
				var openBraceIndent = new Indent(braces.Open);
				var closeBraceIndent = new Indent(braces.Close);
				if (openBraceIndent.IndentedTokenIsFirstAtLine && openBraceIndent.LengthInSpaces != closeBraceIndent.LengthInSpaces)
				{
					yield return BaseStyleValidator.Report(
						braces.Open,
						$"Парные фигурные скобки ({braces}) должны иметь одинаковый отступ.");
				}
			}
		}
	}
}