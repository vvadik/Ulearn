using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class ParametrNameLengthValidator : BaseStyleValidator
	{
		private SyntaxTree tree;

		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution)
		{
			tree = userSolution;
			return InspectAll<FieldDeclarationSyntax>(tree, ReportShortFieldNames)
				.Concat(InspectAll<BaseMethodDeclarationSyntax>(tree, ReportMethodNamesAndParameters));
		}

		private IEnumerable<string> ReportShortFieldNames(FieldDeclarationSyntax fieldDeclarationSyntax)
		{
			var fieldSyntaxToken = fieldDeclarationSyntax.Declaration.Variables.First()?.Identifier;
			if (!fieldSyntaxToken.HasValue
				|| !IsShortName(fieldSyntaxToken.Value.ValueText)
				|| IsContainModifier(fieldDeclarationSyntax, "const")
				|| IsValidName(validShortPublicNames, fieldSyntaxToken.Value.ValueText))
				yield break;
			yield return Report(fieldSyntaxToken.Value, "У поля должно быть более понятное имя");
		}

		private IEnumerable<string> ReportMethodNamesAndParameters(BaseMethodDeclarationSyntax methodDeclarationSyntax)
		{
			if (methodDeclarationSyntax is OperatorDeclarationSyntax || methodDeclarationSyntax is ConversionOperatorDeclarationSyntax)
				yield break;
			
			if (IsShortName(methodDeclarationSyntax.Identifier().ValueText))
				yield return Report(methodDeclarationSyntax.Identifier(), "Имя метода не может состоять из одной буквы");

			foreach (var parameter in methodDeclarationSyntax.ParameterList.Parameters)
			{
				var identifier = parameter.Identifier;
				if (IsShortName(identifier.ValueText) && !IsValidName(validShortPublicNames, identifier.ValueText))
					yield return Report(parameter.Identifier, "У аргументов функции должно быть болле понятное имя");
			}
		}

		private bool IsShortName(string name) => name.Length == 1;

		private bool IsContainModifier(FieldDeclarationSyntax fieldDeclarationSyntax, string modifier) =>
			fieldDeclarationSyntax.Modifiers.Any(x => x.ValueText == modifier);

		private bool IsValidName(IEnumerable<string> validNames, string checkingName) =>
			validNames.Any(name => name.Equals(checkingName, StringComparison.OrdinalIgnoreCase));

		private readonly string[] validShortPublicNames = { "X", "Y", "Z" };
		private readonly string[] validShortPrivateNames = { "k", "l", "m", "n" };

		//FieldDeclarationSntax  -> Modifiers -> Results -> PublicKeyword
	}
}