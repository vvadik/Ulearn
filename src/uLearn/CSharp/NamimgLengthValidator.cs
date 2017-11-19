using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class NamimgLengthValidator : BaseStyleValidator
	{
		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution, SemanticModel semanticModel)
		{
			return InspectAll<FieldDeclarationSyntax>(userSolution, semanticModel, ReportField)
				.Concat(InspectAll<PropertyDeclarationSyntax>(userSolution, semanticModel, ReportProperty))
				.Concat(InspectAll<BaseMethodDeclarationSyntax>(userSolution, semanticModel, ReportMethod));
		}

		private IEnumerable<string> ReportProperty(PropertyDeclarationSyntax propertyDeclarationSyntax, SemanticModel semanticModel)
		{
			var syntaxNodes = propertyDeclarationSyntax.Identifier;
			if (!IsCorrectName(syntaxNodes.ValueText, validCoordinateNames))
				yield return Report(syntaxNodes, "Свойство");
		}

		private IEnumerable<string> ReportField(FieldDeclarationSyntax fieldDeclarationSyntax, SemanticModel semanticModel)
		{
			if (IsContainConstModifier(fieldDeclarationSyntax))
				return new List<string>();

			var syntaxTokens = fieldDeclarationSyntax.Declaration.Variables.Select(declarationSyntax => declarationSyntax.Identifier);
			return ReportSyntaxTokensNames(syntaxTokens, "Поле");
		}

		private IEnumerable<string> ReportMethod(BaseMethodDeclarationSyntax methodDeclarationSyntax, SemanticModel semanticModel) =>
			ReportMethodsParameters(methodDeclarationSyntax.ParameterList)
				.Concat(ReportMethodBody(methodDeclarationSyntax.Body, semanticModel));

		private IEnumerable<string> ReportMethodsParameters(ParameterListSyntax parameterListSyntax)
		{
			var syntaxTokens = parameterListSyntax.Parameters.Select(parametr => parametr.Identifier);
			return ReportSyntaxTokensNames(syntaxTokens, "Аргумент функции");
		}

		private IEnumerable<string> ReportMethodBody(BlockSyntax blockSyntax, SemanticModel semanticModel)
		{
			if (blockSyntax == null)
				return new List<string>();

			var nodes = blockSyntax.DescendantNodes();
			var forStatementSyntaxs = nodes.OfType<ForStatementSyntax>().SelectMany(x => ReportForStatment(x, semanticModel));
			return forStatementSyntaxs.Concat(ReportLovalVariables(nodes, semanticModel));
		}

		private IEnumerable<string> ReportLovalVariables(IEnumerable<SyntaxNode> nodes, SemanticModel semanticModel)
		{
			var declaredSymbols = nodes.OfType<VariableDeclarationSyntax>()
				.Where(IsCorrectType)
				.SelectMany(x => x.Variables)
				.ToDictionary(x => x.Identifier, x => (ILocalSymbol)semanticModel.GetDeclaredSymbol(x));

			var parametersSyntaxTokens = nodes.OfType<ParameterSyntax>().Select(x => x.Identifier);

			return ReportSyntaxTokensNames(declaredSymbols, "Переменная", validLocalNames)
				.Concat(ReportSyntaxTokensNames(parametersSyntaxTokens, "Аргумент LINQ выражения"));
		}

		private bool IsCorrectType(VariableDeclarationSyntax declarationSyntaxs) =>
			!(declarationSyntaxs.Parent is ForStatementSyntax || declarationSyntaxs.Parent is ExpressionStatementSyntax);


		private IEnumerable<string> ReportForStatment(ForStatementSyntax statementSyntax, SemanticModel semanticModel)
		{
			if (statementSyntax.Declaration == null)
				return new List<string>();

			var declaredDictionary = statementSyntax.Declaration.Variables
				.ToDictionary(variable => variable.Identifier, variable => (ILocalSymbol)semanticModel.GetDeclaredSymbol(variable));

			return ReportSyntaxTokensNames(declaredDictionary, "Итератор цикла", validCycleNames)
				.Concat(ReportLovalVariables(statementSyntax.Statement.DescendantNodes(), semanticModel));
		}


		private IEnumerable<string> ReportSyntaxTokensNames(IEnumerable<SyntaxToken> syntaxTokens, string reportMessage, IEnumerable<string> validNodeNames = null)
		{
			var validNames = validNodeNames?.Concat(validCoordinateNames).ToArray() ?? validCoordinateNames;
			foreach (var syntaxToken in syntaxTokens)
			{
				if (!IsCorrectName(syntaxToken.ValueText, validNames))
					yield return Report(syntaxToken, $"{reportMessage} имеет слишком котороткое имя, старайся не использовать однобуквенные названия.");
			}
		}

		private IEnumerable<string> ReportSyntaxTokensNames(IDictionary<SyntaxToken, ILocalSymbol> declaredDictionary, string reportMessage, IEnumerable<string> validNodeNames = null)
		{
			var validNames = validNodeNames?.Concat(validCoordinateNames).ToArray() ?? validCoordinateNames;
			foreach (var declaredPair in declaredDictionary)
			{
				if (!IsCorrectName(declaredPair.Key.ValueText, validNames) && !IsCustomClassStartsWithName(declaredPair.Key.ValueText, declaredPair.Value))
					yield return Report(declaredPair.Key, $"{reportMessage} имеет слишком котороткое имя, старайся не использовать однобуквенные названия.");
			}
		}

		private bool IsCustomClassStartsWithName(string name, ILocalSymbol localSymbol)
		{
			var type = localSymbol.Type;
			if (type == null)
				return false;
			return type.ContainingNamespace?.Name != "System" && type.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase);
		}

		private bool IsCorrectName(string name, IEnumerable<string> validNames) =>
			name.Count(char.IsLetter) > 1 || IsValidName(validNames, name);

		private bool IsContainConstModifier(FieldDeclarationSyntax fieldDeclarationSyntax) =>
			fieldDeclarationSyntax.Modifiers.Any(x => x.ValueText == "const");

		private bool IsValidName(IEnumerable<string> validNames, string checkingName) =>
			validNames.Any(name => name.Equals(checkingName, StringComparison.OrdinalIgnoreCase));

		private readonly string[] validCoordinateNames = { "x", "y", "z" };
		private readonly string[] validCycleNames = { "k", "i", "j", "l" };
		private readonly string[] validLocalNames = { "m", "n" };
	}
}