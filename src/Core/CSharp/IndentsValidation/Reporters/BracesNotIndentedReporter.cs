using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace uLearn.CSharp.IndentsValidation.Reporters
{
	internal static class BracesNotIndentedReporter
	{
		public static IEnumerable<string> Report(BracesPair[] bracesPairs)
		{
			foreach (var braces in bracesPairs.Where(pair => pair.Open.GetLine() != pair.Close.GetLine() &&
															Indent.TokenIsFirstAtLine(pair.Open)))
			{
				var correctOpenbraceParent = braces.Open.GetFirstTokenOfCorrectOpenbraceParent();
				if (correctOpenbraceParent == default(SyntaxToken))
					continue;
				var parentLineIndent = new Indent(correctOpenbraceParent);
				var openbraceLineIndent = new Indent(braces.Open);
				if (openbraceLineIndent.LengthInSpaces < parentLineIndent.LengthInSpaces)
					yield return BaseStyleValidator.Report(braces.Open,
						$"Парные фигурные скобки ({braces}) должны иметь отступ не меньше, чем у родителя.");
			}
		}
	}
}