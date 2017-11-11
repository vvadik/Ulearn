using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class NamimgLengthValidator : BaseStyleValidator
	{
		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution)
		{
			return InspectAll<FieldDeclarationSyntax>(userSolution, ReportField)
				.Concat(InspectAll<PropertyDeclarationSyntax>(userSolution, ReportProperty))
				.Concat(InspectAll<BaseMethodDeclarationSyntax>(userSolution, ReportMethod));
		}

		private IEnumerable<string> ReportProperty(PropertyDeclarationSyntax propertyDeclarationSyntax)
		{
			var syntaxNodes = propertyDeclarationSyntax.Identifier;
			if (!IsCorrectName(syntaxNodes.ValueText, validCoordinateNames))
				yield return Report(syntaxNodes, "Свойство");
		}

		private IEnumerable<string> ReportField(FieldDeclarationSyntax fieldDeclarationSyntax)
		{
			if (IsContainConstModifier(fieldDeclarationSyntax))
				return new List<string>();

			var syntaxTokens = fieldDeclarationSyntax.Declaration.Variables.Select(variableDeclarationSyntax => variableDeclarationSyntax.Identifier);
			return ReportSyntaxTokensNames(syntaxTokens, "Поле");
		}

		private IEnumerable<string> ReportMethod(BaseMethodDeclarationSyntax methodDeclarationSyntax) =>
			ReportMethodsParameters(methodDeclarationSyntax.ParameterList)
				.Concat(ReportMethodBody(methodDeclarationSyntax.Body));

		private IEnumerable<string> ReportMethodsParameters(ParameterListSyntax parameterListSyntax)
		{
			var syntaxTokens = parameterListSyntax.Parameters.Select(parametr => parametr.Identifier);
			return ReportSyntaxTokensNames(syntaxTokens, "Аргумент функции");
		}

		private IEnumerable<string> ReportMethodBody(BlockSyntax blockSyntax)
		{
			var list = new List<string>();

			if (blockSyntax == null)
				return list;

			foreach (var statement in blockSyntax.Statements)
			{
				if (statement is ExpressionStatementSyntax)
					list.AddRange(ReportExpressionStatments(statement as ExpressionStatementSyntax));
				else if (statement is ForStatementSyntax)
					list.AddRange(ReportForStatment(statement as ForStatementSyntax));
				else
					list.AddRange(ReportLovalVariables(statement));
			}

			return list;
		}

		private IEnumerable<string> ReportLovalVariables(StatementSyntax statement)
		{
			var syntaxTokens = statement.DescendantNodes().OfType<VariableDeclarationSyntax>().SelectMany(declarationSynax => declarationSynax.Variables).Select(variableDeclarationSyntax => variableDeclarationSyntax.Identifier);
			return ReportSyntaxTokensNames(syntaxTokens, "Локальная переменная", validLocalNames);
		}

		private IEnumerable<string> ReportForStatment(ForStatementSyntax statementSyntax)
		{
			if (statementSyntax.Declaration == null)
				return new List<string>();
			var syntaxTokens = statementSyntax.Declaration.Variables.Select(variable => variable.Identifier);
			return ReportSyntaxTokensNames(syntaxTokens, "Итератор цикла", validCycleNames);
		}

		private IEnumerable<string> ReportExpressionStatments(ExpressionStatementSyntax expressionStatementSyntax)
		{
			var syntaxTokens = expressionStatementSyntax.DescendantNodes().OfType<ParameterSyntax>().Select(parameterSyntax => parameterSyntax.Identifier);
			return ReportSyntaxTokensNames(syntaxTokens, "Аргумент LINQ выражения");
		}


		private IEnumerable<string> ReportSyntaxTokensNames(IEnumerable<SyntaxToken> nodesToCheck, string reportMessage, IEnumerable<string> validNodeNames = null)
		{
			var validNames = validNodeNames?.Concat(validCoordinateNames).ToArray() ?? validCoordinateNames;
			foreach (var syntaxToken in nodesToCheck)
			{
				if (!IsCorrectName(syntaxToken.ValueText, validNames))
					yield return Report(syntaxToken, $"{reportMessage} имеет слишком котороткое имя, старайся не использовать однобуквенные названия.");
			}
		}

		private bool IsCorrectName(string name, IEnumerable<string> validNames) =>
			name.Length != 1 || IsValidName(validNames, name);

		private bool IsContainConstModifier(FieldDeclarationSyntax fieldDeclarationSyntax) =>
			fieldDeclarationSyntax.Modifiers.Any(x => x.ValueText == "const");

		private bool IsValidName(IEnumerable<string> validNames, string checkingName) =>
			validNames.Any(name => name.Equals(checkingName, StringComparison.OrdinalIgnoreCase));

		private readonly string[] validCoordinateNames = { "x", "y", "z" };
		private readonly string[] validCycleNames = { "k", "i", "j", "l" };
		private readonly string[] validLocalNames = { "m", "n" };
	}
}