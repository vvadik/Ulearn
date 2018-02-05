using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace uLearn.CSharp
{
	public abstract class BaseStyleValidatorWithSemanticModel: BaseStyleValidator
	{
		private static readonly PortableExecutableReference mscorlib =
			MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

		protected override IEnumerable<string> ReportAllErrors(SyntaxTree userSolution)
		{
			var compilation = CSharpCompilation.Create("MyCompilation", new[] { userSolution },
				new[] { mscorlib });
			var semanticModel = compilation.GetSemanticModel(userSolution);
			return ReportAllErrors(userSolution, semanticModel); 
		}
		
		protected IEnumerable<string> InspectAll<TNode>(SyntaxTree userSolution, SemanticModel semanticModel,
			Func<TNode, SemanticModel, IEnumerable<string>> inspect)
			where TNode : SyntaxNode
		{
			var nodes = userSolution.GetRoot().DescendantNodes().OfType<TNode>();
			return nodes.SelectMany(n => inspect(n, semanticModel));
		}

		protected abstract IEnumerable<string> ReportAllErrors(SyntaxTree userSolution, SemanticModel semanticModel);
	}
}