using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public static class ArrayLengthStylePrimitives
	{
		public static bool IsCycle(this SyntaxNode syntaxNode)
		{
			return syntaxNode is ForStatementSyntax
					|| syntaxNode is WhileStatementSyntax
					|| syntaxNode is DoStatementSyntax
					|| syntaxNode is ForEachStatementSyntax;
		}

		public static bool ContainsAssignmentOf(this StatementSyntax statementSyntax, string variableName,
			SemanticModel semanticModel) // TODO: посмотреть насчёт передачи списка узлов
		{ // TODO: сделать проверку на массивы (не здесь)
			if (!statementSyntax.IsCycle())
				return false;

			var assignments = statementSyntax
				.DescendantNodes()
				.OfType<AssignmentExpressionSyntax>()
				.ToList();
			var declarations = statementSyntax
				.DescendantNodes()
				.OfType<VariableDeclaratorSyntax>()
				.ToList();
			
			foreach (var assignment in assignments)
			{
				var variable = assignment
					.GetAllDescendantVariables()
					.FirstOrDefault();
				if (variable.HasName(variableName, semanticModel))
					return true;
			}
			foreach (var declaration in declarations)
			{
				var variable = declaration.Identifier;
				if (variable.Text == variableName)
					return true;
			}
			
			return false;
		}

		public static List<IdentifierNameSyntax> GetAllDescendantVariables(this SyntaxNode node)
		{
			return node
				.DescendantNodes()
				.OfType<IdentifierNameSyntax>()
				.ToList();
		}

		public static bool HasName(this SyntaxNode node, string variableName,
			SemanticModel semanticModel)
		{
			var symbol = semanticModel.GetSymbolInfo(node).Symbol;
			return symbol?.ToString() == variableName;
		}

		public static bool HasVariableAsArgument(this InvocationExpressionSyntax methodInvocation,
			string variableName, SemanticModel semanticModel)
		{
			var arguments = methodInvocation.ArgumentList.Arguments;
			return arguments
				.Select(v => semanticModel.GetSymbolInfo(v).Symbol)
				.Any(s => s?.ToString() == variableName);
		}
	}
}