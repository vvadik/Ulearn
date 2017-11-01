using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class BoolCompareValidator : BaseStyleValidator
	{
		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution)
		{
			var compilation = CSharpCompilation.Create("MyCompilation", new[] { userSolution }, new[] { mscorlib });
			var semanticModel = compilation.GetSemanticModel(userSolution);
			var nodes = userSolution.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>();
			return nodes.SelectMany(node => Inspect(semanticModel, node));
		}

		private IEnumerable<string> Inspect(SemanticModel semanticModel, BinaryExpressionSyntax binaryExpression)
		{
			var operatorKind = binaryExpression.OperatorToken.Kind();
			if (operatorKind != SyntaxKind.EqualsEqualsToken && operatorKind != SyntaxKind.ExclamationEqualsToken)
				yield break;
			var leftNodeTypeInfo = semanticModel.GetTypeInfo(binaryExpression.Left);
			var leftNodeTree = binaryExpression.Left as LiteralExpressionSyntax;
			var rightNodeTypeInfo = semanticModel.GetTypeInfo(binaryExpression.Right);
			var rightNodeTree = binaryExpression.Right as LiteralExpressionSyntax;
			if (IsBooleanType(rightNodeTypeInfo) && IsBooleanType(leftNodeTypeInfo) 
				&& (IsBoolLiteral(leftNodeTree) || IsBoolLiteral(rightNodeTree)))
				yield return Report(binaryExpression, "Ненужное сравнение с переменной типа bool. Вместо x == true лучше писать просто x, а вместо x != true лучше писать !x.");
		}

		private static bool IsBoolLiteral(LiteralExpressionSyntax node)
		{
			if (node == null)
				return false;
			var token = node.Token.Kind();
			return token == SyntaxKind.TrueKeyword || token == SyntaxKind.FalseKeyword;
		}

		private bool IsBooleanType(TypeInfo node) => node.Type?.Name == "Boolean";

		private readonly PortableExecutableReference mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
	}
}