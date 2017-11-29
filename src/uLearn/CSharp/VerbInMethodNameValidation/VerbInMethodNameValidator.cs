using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class VerbInMethodNameValidator : BaseStyleValidator
	{
		private readonly HashSet<string> englishVerbs;

		public VerbInMethodNameValidator()
		{
			var lines = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "uLearn\\Csharp\\VerbInMethodNameValidation\\31K verbs.txt"));
			englishVerbs = new HashSet<string>(lines);
		}

		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution, SemanticModel semanticModel)
		{
			return InspectAll<MethodDeclarationSyntax>(userSolution, InspectMethodsNames);
		}

		private IEnumerable<string> InspectMethodsNames(MethodDeclarationSyntax methodDeclarationSyntax)
		{
			var syntaxToken = methodDeclarationSyntax.Identifier();
			if (exceptions.Contains(syntaxToken.ValueText))
				yield break;

			var wordsInName = SplitMethodName(syntaxToken.ValueText);

			foreach (var word in wordsInName)
			{
				if (exceptions.Contains(word) || englishVerbs.Any(x => x.Equals(word, StringComparison.InvariantCultureIgnoreCase)))
					yield break;
			}
			yield return Report(syntaxToken, "В названии метода отсутствует глагол");
		}

		private IEnumerable<string> SplitMethodName(string methodName)
		{
			var word = "";
			foreach (var letter in methodName)
			{
				if (char.IsUpper(letter) && word != "")
				{
					yield return word;
					word = "";
				}
				word += letter;
			}
			yield return word;
		}

		private IEnumerable<string> exceptions = new[] { "Main"};
	}
}