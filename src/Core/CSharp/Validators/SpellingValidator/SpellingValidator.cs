using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NHunspell;
using Ulearn.Common.Extensions;
using Ulearn.Core.CSharp;
using Ulearn.Core.CSharp.Validators;
using Ulearn.Core.Model.Edx.EdxComponents;
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
			var errors = InspectMethodParameters(methodDeclaration);
			var errorInMethodName = CheckIdentifierNameForSpellingErrors(
				methodIdentifier,
				methodDeclaration.ReturnType.ToString());
			if (errorInMethodName != null)
				errors.Add(errorInMethodName);
			return errors;
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
			return CheckIdentifierNameForSpellingErrors(identifier, parameter.Type.ToString());
		}

		private IEnumerable<SolutionStyleError> InspectVariablesNames(VariableDeclarationSyntax variableDeclarationSyntax, SemanticModel semanticModel)
		{
			var typeInfo = semanticModel.GetTypeInfo(variableDeclarationSyntax.Type);
			return variableDeclarationSyntax.Variables.SelectMany(v => InspectVariablesNames(v, typeInfo));
		}

		private IEnumerable<SolutionStyleError> InspectClassNames(ClassDeclarationSyntax classDeclarationSyntax)
		{
			var errors = new List<SolutionStyleError>();
			var errorInName = CheckIdentifierNameForSpellingErrors(classDeclarationSyntax.Identifier);
			if (errorInName != null)
				errors.Add(errorInName);
			return errors;
		}
		
		private IEnumerable<SolutionStyleError> InspectStructNames(StructDeclarationSyntax structDeclarationSyntax)
		{
			var errors = new List<SolutionStyleError>();
			var errorInName = CheckIdentifierNameForSpellingErrors(structDeclarationSyntax.Identifier);
			if (errorInName != null)
				errors.Add(errorInName);
			return errors;
		}
		
		private IEnumerable<SolutionStyleError> InspectInterfaceNames(InterfaceDeclarationSyntax interfaceDeclarationSyntax)
		{
			var interfaceNameParts = interfaceDeclarationSyntax.Identifier.ValueText.SplitByCamelCase();
			var errors = new List<SolutionStyleError>();
			if (interfaceNameParts.First() != "I")
				errors.Add(new SolutionStyleError(
					StyleErrorType.Misspeling01,
					interfaceDeclarationSyntax.Identifier,
					$"Имя интерфейса должно начинаться с I"));
			var errorInName = CheckIdentifierNameForSpellingErrors(interfaceDeclarationSyntax.Identifier);
			if (errorInName != null)
				errors.Add(errorInName);
			return errors;
		}

		private IEnumerable<SolutionStyleError> InspectVariablesNames(VariableDeclaratorSyntax variableDeclaratorSyntax, TypeInfo variableTypeInfo)
		{
			var errors = new List<SolutionStyleError>();
			var errrorInVariable = CheckIdentifierNameForSpellingErrors(variableDeclaratorSyntax.Identifier, variableTypeInfo.Type.Name);
			if (errrorInVariable != null)
				errors.Add(errrorInVariable);
			return new List<SolutionStyleError>();
		}

		private IEnumerable<SolutionStyleError> InspectPropertiesNames(PropertyDeclarationSyntax propertyDeclaration)
		{
			var errors = new List<SolutionStyleError>();
			var propertyType = propertyDeclaration.Type;
			var propertyTypeAsString = propertyType.ToString();
			var errorInPropertyName = CheckIdentifierNameForSpellingErrors(propertyDeclaration.Identifier, propertyTypeAsString);
			if (errorInPropertyName != null)
				errors.Add(errorInPropertyName);
			
			return errors;
		}

		private SolutionStyleError CheckIdentifierNameForSpellingErrors(SyntaxToken identifier, string typeAsString = null)
		{
			var identifierName = identifier.ValueText;
			return spellChecker.CheckIdentifierNameForSpellingErrors(identifierName, typeAsString)
				? new SolutionStyleError(StyleErrorType.Misspeling01, identifier, $"В слове {identifierName} допущена опечатка.")
				: null;
		}
	}
}