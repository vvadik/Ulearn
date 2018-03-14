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
				if (!hunspell.Spell(word))
					yield return new SolutionStyleError(identifier, $"В слове {word} допущена опечатка. Возможные исправления: {string.Join(", ", hunspell.Suggest(word))}");
			}
		}
	}
}