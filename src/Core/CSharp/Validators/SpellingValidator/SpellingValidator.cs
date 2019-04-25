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
		private readonly CodeElementSpellChecker spellChecker = new CodeElementSpellChecker();
		public override List<SolutionStyleError> FindErrors(SyntaxTree userSolution, SemanticModel semanticModel)
		{	
			return InspectAll<MethodDeclarationSyntax>(userSolution, InspectMethodsNamesAndArguments) // TODO: проверять имена классов?
				.Concat(InspectAll<VariableDeclarationSyntax>(userSolution, semanticModel, InspectVariablesNames))
				.Concat(InspectAll<PropertyDeclarationSyntax>(userSolution, InspectPropertiesNames))
				.Concat(InspectAll<ClassDeclarationSyntax>(userSolution, InspectClassNames))
				.Concat(InspectAll<StructDeclarationSyntax>(userSolution, InspectStructNames))
				.Concat(InspectAll<InterfaceDeclarationSyntax>(userSolution, InspectInterfaceNames))
				.ToList();
		}

		private IEnumerable<SolutionStyleError> InspectMethodsNamesAndArguments(MethodDeclarationSyntax methodDeclaration)
		{
			var methodIdentifier = methodDeclaration.Identifier;
			var errorsInParameters = InspectMethodParameters(methodDeclaration);

			return CheckIdentifierNameForSpellingErrors(
					methodIdentifier,
					methodDeclaration.ReturnType.ToString())
				.Concat(errorsInParameters);
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
			var errorsInParameterName = CheckIdentifierNameForSpellingErrors(identifier, parameter.Type.ToString());

			return errorsInParameterName.FirstOrDefault();
		}

		private IEnumerable<SolutionStyleError> InspectVariablesNames(VariableDeclarationSyntax variableDeclarationSyntax, SemanticModel semanticModel)
		{
			var typeInfo = semanticModel.GetTypeInfo(variableDeclarationSyntax.Type);
			return variableDeclarationSyntax.Variables.SelectMany(v => InspectVariablesNames(v, typeInfo));
		}

		private IEnumerable<SolutionStyleError> InspectClassNames(ClassDeclarationSyntax classDeclarationSyntax)
		{
			return CheckIdentifierNameForSpellingErrors(classDeclarationSyntax.Identifier);
		}
		
		private IEnumerable<SolutionStyleError> InspectStructNames(StructDeclarationSyntax structDeclarationSyntax)
		{
			return CheckIdentifierNameForSpellingErrors(structDeclarationSyntax.Identifier);
		}
		
		private IEnumerable<SolutionStyleError> InspectInterfaceNames(InterfaceDeclarationSyntax interfaceDeclarationSyntax)
		{
			var interfaceNameParts = interfaceDeclarationSyntax.Identifier.ValueText.SplitByCamelCase();
			var errorInInterfaceSymbol = new List<SolutionStyleError>();
			if (interfaceNameParts.First() != "I")
				errorInInterfaceSymbol.Add(new SolutionStyleError(
					StyleErrorType.Misspeling01,
					interfaceDeclarationSyntax.Identifier,
					$"Имя интерфейса должно начинаться с I"));
			return errorInInterfaceSymbol.Concat(CheckIdentifierNameForSpellingErrors(interfaceDeclarationSyntax.Identifier));
		}

		private IEnumerable<SolutionStyleError> InspectVariablesNames(VariableDeclaratorSyntax variableDeclaratorSyntax, TypeInfo variableTypeInfo)
		{
			return CheckIdentifierNameForSpellingErrors(variableDeclaratorSyntax.Identifier, variableTypeInfo.Type.Name);
		}

		private IEnumerable<SolutionStyleError> InspectPropertiesNames(PropertyDeclarationSyntax propertyDeclaration)
		{
			var propertyType = propertyDeclaration.Type;
			var propertyTypeAsString = propertyType.ToString();

			return CheckIdentifierNameForSpellingErrors(propertyDeclaration.Identifier, propertyTypeAsString);
		}

		private IEnumerable<SolutionStyleError> CheckIdentifierNameForSpellingErrors(SyntaxToken identifier, string typeAsString = null)
		{
			return spellChecker.CheckIdentifierNameForSpellingErrors(identifier, typeAsString);
		}
	}
}