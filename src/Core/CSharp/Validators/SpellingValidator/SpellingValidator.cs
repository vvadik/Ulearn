using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NHunspell;
using Ulearn.Common.Extensions;
using Ulearn.Core.CSharp;
using Ulearn.Core.CSharp.Validators;
using Ulearn.Core.Properties;

namespace uLearn.CSharp.Validators.SpellingValidator
{
	public class SpellingValidator: BaseStyleValidator
	{
		private static readonly Hunspell hunspell = new Hunspell(Resources.en_US_aff, Resources.en_US_dic);
		private static readonly HashSet<string> wordsToExcept = new HashSet<string>
		{
			"func", "arg", "args", "pos", "sw", "bmp", "prev", "next", "rnd", "ui", "autocomplete", "tuple", "len", "api", "tuples", "vm",
			"ai"
		};
		
		public override List<SolutionStyleError> FindErrors(SyntaxTree userSolution, SemanticModel semanticModel)
		{	
			return InspectAll<MethodDeclarationSyntax>(userSolution, InspectMethodsNamesAndArguments)
				.Concat(InspectAll<VariableDeclarationSyntax>(userSolution, semanticModel, InspectVariablesNames))
				.Concat(InspectAll<PropertyDeclarationSyntax>(userSolution, InspectPropertiesNames))
				.ToList();
		}

		private IEnumerable<SolutionStyleError> InspectMethodsNamesAndArguments(MethodDeclarationSyntax methodDeclaration)
		{
			var methodIdentifier = methodDeclaration.Identifier;
			var errorsInParameters = InspectMethodParameters(methodDeclaration);

			return CheckIdentifierNameForSpellingErrors(methodIdentifier, methodDeclaration.ReturnType.ToString()).Concat(errorsInParameters);
		}

		private List<SolutionStyleError> InspectMethodParameters(MethodDeclarationSyntax methodDeclaration)
		{
			var parameters = methodDeclaration.ParameterList.Parameters;
			return parameters
				.Select(InspectMethodParameter)
				.Where(err => err != null)
				.ToList();
		}

		private SolutionStyleError InspectMethodParameter(ParameterSyntax parameter)
		{
			var identifier = parameter.Identifier;
			var parameterTypeAsString = parameter.Type.ToString();
			var errorsInParameterName = CheckIdentifierNameForSpellingErrors(identifier, parameterTypeAsString);

			return errorsInParameterName.FirstOrDefault();
		}

		private IEnumerable<SolutionStyleError> InspectVariablesNames(VariableDeclarationSyntax variableDeclarationSyntax, SemanticModel semanticModel)
		{
			var typeInfo = semanticModel.GetTypeInfo(variableDeclarationSyntax.Type);
			return variableDeclarationSyntax.Variables.SelectMany(v => InspectVariablesNames(v, typeInfo));
		}

		private IEnumerable<SolutionStyleError> InspectVariablesNames(VariableDeclaratorSyntax variableDeclaratorSyntax, TypeInfo variableTypeInfo)
		{
			var variableIdentifier = variableDeclaratorSyntax.Identifier;
			var variableType = variableTypeInfo.Type;
			var variableTypeName = variableType.Name;
			// var variableName = variableIdentifier.Text;
			// if (variableTypeName.StartsWith(variableName, StringComparison.InvariantCultureIgnoreCase) //
			// 	|| variableTypeName.MakeTypeNameAbbreviation().Equals(variableName, StringComparison.InvariantCultureIgnoreCase)) // TODO: перетащить проверку в CheckIdentifierName?
			// 	return new List<SolutionStyleError>();
			
			return CheckIdentifierNameForSpellingErrors(variableIdentifier, variableTypeName);
		}

		private IEnumerable<SolutionStyleError> InspectPropertiesNames(PropertyDeclarationSyntax propertyDeclaration)
		{
			var propertyType = propertyDeclaration.Type;
			var propertyTypeAsString = propertyType.ToString();
			var propertyName = propertyDeclaration.Identifier.Text;
			// if (propertyTypeAsString.StartsWith(propertyName, StringComparison.InvariantCultureIgnoreCase)
			// 	|| propertyTypeAsString.MakeTypeNameAbbreviation().Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
			// 	return new List<SolutionStyleError>();
			
			return CheckIdentifierNameForSpellingErrors(propertyDeclaration.Identifier, propertyTypeAsString);
		}

		private IEnumerable<SolutionStyleError> CheckIdentifierNameForSpellingErrors(SyntaxToken identifier, string typeAsString)
		{
			var wordsInIdentifier = identifier.ValueText.SplitByCamelCase();
			foreach (var word in wordsInIdentifier)
			{
				var wordForCheck = RemoveIfySuffix(word.ToLowerInvariant());
				var wordInDifferentNumbers = GetWordInDifferentNumbers(wordForCheck);
				var doesWordContainError = true;
				foreach (var wordInDifferentNumber in wordInDifferentNumbers)
				{
					if (wordInDifferentNumber.Length <= 2
						|| typeAsString.StartsWith(wordInDifferentNumber, StringComparison.InvariantCultureIgnoreCase) // TODO: проверять множественное число
						|| wordInDifferentNumber.Equals(typeAsString.MakeTypeNameAbbreviation(), StringComparison.InvariantCultureIgnoreCase) // TODO: сделать и для небольших частей?
						|| wordsToExcept.Contains(wordForCheck) || hunspell.Spell(wordInDifferentNumber))
					{
						doesWordContainError = false;
						break;
					}
					var possibleErrorInWord = CheckConcatenatedWordsInLowerCaseForError(wordInDifferentNumber, identifier);
					if (possibleErrorInWord != null)
						continue;
					doesWordContainError = false;
					break;
				}
				if (doesWordContainError)
					yield return new SolutionStyleError(StyleErrorType.Misspeling01, identifier, $"В слове {word} допущена опечатка.");
			}
		}

		private string RemoveIfySuffix(string word)
		{
			return word.LastIndexOf("ify", StringComparison.InvariantCultureIgnoreCase) > 0
				? word.Substring(0, word.Length - 3)
				: word;
		}

		private IEnumerable<string> GetWordInDifferentNumbers(string word)
		{
			yield return word;
			if (word.Length > 1 && word.EndsWith("s", StringComparison.InvariantCultureIgnoreCase))
				yield return word.Substring(0, word.Length - 1);
			if (word.Length > 2 && word.EndsWith("es", StringComparison.InvariantCultureIgnoreCase))
				yield return word.Substring(0, word.Length - 2);
		}

		private SolutionStyleError CheckConcatenatedWordsInLowerCaseForError(string concatenatedWords, SyntaxToken tokenWithConcatenatedWords)
		{
			var currentCheckingWord = "";
			foreach (var symbol in concatenatedWords)
			{
				currentCheckingWord += symbol;
				if (currentCheckingWord == "I" || currentCheckingWord.Length != 1 && hunspell.Spell(currentCheckingWord))
					currentCheckingWord = "";
			}

			return currentCheckingWord != ""
				? new SolutionStyleError(StyleErrorType.Misspeling01, tokenWithConcatenatedWords, $"В слове {concatenatedWords} допущена опечатка.")
				: null;
		}
	}
}