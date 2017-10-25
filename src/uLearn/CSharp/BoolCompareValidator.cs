using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RazorEngine.Compilation.ImpromptuInterface.Optimization;
using uLearn.Extensions;

namespace uLearn.CSharp
{
	public class BoolCompareValidator : BaseStyleValidator
	{
		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution)
		{
			var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
			var compilation = CSharpCompilation.Create("MyCompilation", new[] { userSolution }, new[] { mscorlib });
			semanticModel = compilation.GetSemanticModel(userSolution);
			return InspectAll<BinaryExpressionSyntax>(userSolution, Inspect);
		}

		private IEnumerable<string> Inspect(BinaryExpressionSyntax binaryExpression)
		{
			var leftNodeModel = semanticModel.GetTypeInfo(binaryExpression.Left);
			var leftNodeTree = binaryExpression.Left.Call(n => n as LiteralExpressionSyntax);
			var rightNodeModel = semanticModel.GetTypeInfo(binaryExpression.Right);
			var rightNodeTree = binaryExpression.Right.Call(n => n as LiteralExpressionSyntax);
			if (IsBooleanType(rightNodeModel) && IsBooleanType(leftNodeModel) && (IsBoolLiteral(leftNodeTree) || IsBoolLiteral(rightNodeTree)))
				yield return Report(binaryExpression, "Ненужное сравнение с переменной типа bool");
		}

		private static bool IsBoolLiteral(LiteralExpressionSyntax node)
		{
			if (node == null)
				return false;
			var token = node.Token.Kind();
			return token == SyntaxKind.TrueKeyword || token == SyntaxKind.FalseKeyword;
		}

		private bool IsBooleanType(TypeInfo node)
		{
			return node.Type?.Name == "Boolean"; //Boolean
		}

		private SemanticModel semanticModel;
	}
}