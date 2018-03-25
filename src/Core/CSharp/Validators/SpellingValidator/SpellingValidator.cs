using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NHunspell;
using uLearn.Properties;
using Ulearn.Common.Extensions;

namespace uLearn.CSharp.Validators.SpellingValidator
{
	public class SpellingValidator: BaseStyleValidator
	{
		private static readonly Hunspell hunspell = new Hunspell(Resources.en_US_aff, Resources.en_US_dic);
		private static readonly HashSet<string> wordsToExcept = new HashSet<string>
		{
			"func", "arg", "args", "pos", "sw", "bmp", "prev", "next", "rnd", "ui", "autocomplete", "tuple", "len", "api", "tuples", "vm"
		};
		
		public override List<SolutionStyleError> FindErrors(SyntaxTree userSolution, SemanticModel semanticModel)
		{
			return InspectAll<MethodDeclarationSyntax>(userSolution, InspectMethodsNamesAndArguments)
				.Concat(InspectAll<VariableDeclaratorSyntax>(userSolution, InspectVariablesNames))
				.Concat(InspectAll<PropertyDeclarationSyntax>(userSolution, InspectPropertiesNames))
				.ToList();
		}

		private IEnumerable<SolutionStyleError> InspectMethodsNamesAndArguments(MethodDeclarationSyntax methodDeclaration)
		{
			var methodIdentifier = methodDeclaration.Identifier;
			var parameters = methodDeclaration.ParameterList.Parameters;
			return CheckIdentifierNameForSpellingErrors(methodIdentifier)
				.Concat(parameters
					.SelectMany(p => CheckIdentifierNameForSpellingErrors(p.Identifier)));
		}

		private IEnumerable<SolutionStyleError> InspectVariablesNames(VariableDeclaratorSyntax variableDeclaratorSyntax)
		{
			var variableDeclarator = variableDeclaratorSyntax.Identifier;
			return CheckIdentifierNameForSpellingErrors(variableDeclarator);
		}

		private IEnumerable<SolutionStyleError> InspectPropertiesNames(PropertyDeclarationSyntax propertyDeclaration)
		{
			return CheckIdentifierNameForSpellingErrors(propertyDeclaration.Identifier);
		}

		private IEnumerable<SolutionStyleError> CheckIdentifierNameForSpellingErrors(SyntaxToken identifier)
		{
			var wordsInIdentifier = identifier.ValueText.SplitByCamelCase();
			foreach (var word in wordsInIdentifier)
			{
				if (!wordsToExcept.Contains(word.ToLowerInvariant()) && !hunspell.Spell(word))
				{
					var possibleErrorInWord = CheckConcatenatedWordsInLowerCaseForError(word, identifier);
					if (possibleErrorInWord != null)
						yield return possibleErrorInWord;
				}
			}
		}

		private SolutionStyleError CheckConcatenatedWordsInLowerCaseForError(
			string concatenatedWords,
			SyntaxToken tokenWithConcatenatedWords
			)
		{
			//var biggestUncheckedPrefix = concatenatedWords;
			//var biggestValidWordsSequence = "";
			//while (!string.IsNullOrWhiteSpace(biggestUncheckedPrefix))
			//{
			//	if (hunspell.Spell(biggestUncheckedPrefix))
			//	{
			//		var prefixStartPosition = concatenatedWords.IndexOf(biggestUncheckedPrefix);
			//		var nextWordStartPosition = prefixStartPosition + biggestUncheckedPrefix.Length;
			//		biggestValidWordsSequence += biggestUncheckedPrefix;
			//		biggestUncheckedPrefix = concatenatedWords.Substring(nextWordStartPosition);
			//	}
			//	else
			//	{
			//		biggestUncheckedPrefix = biggestUncheckedPrefix.Remove(biggestUncheckedPrefix.Length - 1);
			//	}
			//}

			//if (biggestValidWordsSequence != concatenatedWords)
			//{
			//	var incorrectWordStartPosition = concatenatedWords.Length - biggestValidWordsSequence.Length;
			//	var incorrectWord = concatenatedWords.Substring(incorrectWordStartPosition);
			//	return new SolutionStyleError(tokenWithConcatenatedWords, $"В слове {incorrectWord} допущена опечатка. Возможные исправления: {string.Join(", ", hunspell.Suggest(incorrectWord))}");
			//}
			//return null;
			var currentCheckingWord = "";
			foreach (var symbol in concatenatedWords)
			{
				currentCheckingWord += symbol;
				if (currentCheckingWord == "I" || currentCheckingWord.Length != 1 && hunspell.Spell(currentCheckingWord))
					currentCheckingWord = "";
			}

			return currentCheckingWord != ""
				? new SolutionStyleError(tokenWithConcatenatedWords, $"В слове {currentCheckingWord} допущена опечатка. Возможные исправления: {string.Join(", ", hunspell.Suggest(currentCheckingWord))}")
				: null;
		}
	}
}