using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class RedundantIfStyleValidator : BaseStyleValidator
	{
		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution)
		{
			return InspectAll<IfStatementSyntax>(userSolution, Inspect);
		}

		public IEnumerable<string> Inspect(IfStatementSyntax ifElseStatement)
		{
			var trueReturnValue =
				(ifElseStatement.Statement as ReturnStatementSyntax)
					.Call(r => r.Expression as LiteralExpressionSyntax)
					.Call(IsBoolLiteral, null);
			var falseReturnValue =
				ifElseStatement.Else
					.Call(e => e.Statement as ReturnStatementSyntax)
					.Call(r => r.Expression as LiteralExpressionSyntax)
					.Call(IsBoolLiteral, null);

			var nextSibling = ifElseStatement.Parent.ChildNodes().SkipWhile(n => n != ifElseStatement).Skip(1).FirstOrDefault();
			falseReturnValue = falseReturnValue ?? 
								(nextSibling as ReturnStatementSyntax)
									.Call(r => r.Expression as LiteralExpressionSyntax)
									.Call(IsBoolLiteral, null);
			if (trueReturnValue != null && falseReturnValue != null)
				yield return Report(ifElseStatement, "Используйте return вместо if");
		}

		private static bool? IsBoolLiteral(LiteralExpressionSyntax node)
		{
			var token = node.Token.CSharpKind();
			return token == SyntaxKind.TrueKeyword || token == SyntaxKind.FalseKeyword;
		}
	}
}