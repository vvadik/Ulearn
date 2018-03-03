using System.Collections.Generic;
using System.Linq;

namespace uLearn.CSharp.IndentsValidation.Reporters
{
	internal static class CloseBraceHasCodeOnSameLineReporter
	{
		public static IEnumerable<string> Report(BracesPair[] bracesPairs)
		{
			foreach (var braces in bracesPairs.Where(pair => IndentsSyntaxExtensions.GetLine(pair.Open) != IndentsSyntaxExtensions.GetLine(pair.Close)))
			{
				var openBraceIndent = new Indent(braces.Open);
				var closeBraceIndent = new Indent(braces.Close);
				if (openBraceIndent.IndentedTokenIsFirstAtLine && !closeBraceIndent.IndentedTokenIsFirstAtLine)
				{
					yield return BaseStyleValidator.Report(braces.Close, "Перед закрывающей фигурной скобкой на той же строке не должно быть кода.");
				}
			}
		}
	}
}