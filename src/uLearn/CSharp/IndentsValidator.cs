using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using uLearn.CSharp;

namespace uLearn
{
	public class IndentsValidator : CSharpSyntaxWalker, ICSharpSolutionValidator
	{
		private string errors;

		public string FindError(SyntaxTree userSolution)
		{
			Visit(userSolution.GetRoot());
			return errors;
		}

		public override void Visit(SyntaxNode node)
		{
			base.Visit(node);
		}
	}
}