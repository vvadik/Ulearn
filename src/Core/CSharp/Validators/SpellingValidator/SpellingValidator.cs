using System;
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
				.Concat(InspectAll<VariableDeclarationSyntax>(userSolution, semanticModel, InspectVariablesNames))
				.Concat(InspectAll<PropertyDeclarationSyntax>(userSolution, InspectPropertiesNames))
				.ToList();
		}

		private IEnumerable<SolutionStyleError> InspectMethodsNamesAndArguments(MethodDeclarationSyntax methodDeclaration)
		{
			var methodIdentifier = methodDeclaration.Identifier;
			var errorsInParameters = InspectMethodParameters(methodDeclaration);

			return CheckIdentifierNameForSpellingErrors(methodIdentifier).Concat(errorsInParameters);
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
			var identifierText = identifier.Text;
			var errorsInParameterName = CheckIdentifierNameForSpellingErrors(identifier);
			foreach (var errorInParameterName in errorsInParameterName)
			{
				var parameterTypeAsString = parameter.Type.ToString();
				if (!parameterTypeAsString.StartsWith(identifierText, StringComparison.InvariantCultureIgnoreCase)
					|| identifierText.Equals(parameterTypeAsString.MakeTypeNameAbbreviation(), StringComparison.InvariantCultureIgnoreCase))
					return errorInParameterName;
			}

			return null;
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
			if (!variableType.Name.StartsWith(variableIdentifier.Text, StringComparison.InvariantCultureIgnoreCase))
				return new List<SolutionStyleError>();
			
			return CheckIdentifierNameForSpellingErrors(variableIdentifier);
		}

		private IEnumerable<SolutionStyleError> InspectPropertiesNames(PropertyDeclarationSyntax propertyDeclaration)
		{
			var propertyType = propertyDeclaration.Type;
			var propertyTypeAsString = propertyType.ToString();
			var propertyTypeNameAbbreviation = propertyTypeAsString.MakeTypeNameAbbreviation();
			if (propertyDeclaration.Identifier.Text.Equals(propertyTypeNameAbbreviation))
				return new List<SolutionStyleError>();
			
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
				? new SolutionStyleError(tokenWithConcatenatedWords, $"В слове {concatenatedWords} допущена опечатка.")
				: null;
		}
	}
}