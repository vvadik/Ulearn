using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class ExerciseBuilder : CSharpSyntaxRewriter
	{
		private readonly string prelude;
		public string ExerciseClassName { get; set; }

		public ExerciseSlide Slide { get; private set; }
		private CSharpSolutionValidator validator;

		public ExerciseBuilder(string prelude, SlideBuilder blocksBuilder, SlideInfo slideInfo)
			: base(false)
		{
			this.prelude = prelude;
			Slide = new ExerciseSlide(blocksBuilder.Blocks, slideInfo, blocksBuilder.Title, blocksBuilder.Id);
		}

		public ExerciseSlide BuildFrom(SyntaxTree tree)
		{
			ExerciseClassName = null;
			var sb = new SolutionBuilder { Validator = validator = new CSharpSolutionValidator() };
			SyntaxNode result = Visit(tree.GetRoot());
			Debug.Assert(ExerciseClassName != null);
			var exerciseClassBodyStartIndex = GetExerciseInsertIndex(result);
			const string pragma = "\n#line 1\n";
			sb.ExerciseCode = prelude + result.ToFullString().Insert(exerciseClassBodyStartIndex, pragma);
			sb.IndexForInsert = prelude.Length + exerciseClassBodyStartIndex + pragma.Length;
			Slide.Solution = sb;
			return Slide;
		}

		private int GetExerciseInsertIndex(SyntaxNode result)
		{
			ClassDeclarationSyntax exerciseClass =
				result.DescendantNodes()
					.OfType<ClassDeclarationSyntax>()
					.FirstOrDefault(n => n.Identifier.Text == ExerciseClassName);
			if (exerciseClass !=null)
				return exerciseClass.OpenBraceToken.Span.End;
			var ns = result.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
			if (ns != null)
				return ns.OpenBraceToken.Span.End;
			return 0;
		}

		public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node)
		{
			return null;
		}

		public static bool IsExercise(SyntaxTree tree)
		{
			return 
				tree.GetRoot()
				.DescendantNodes()
				.OfType<MethodDeclarationSyntax>()
				.Any(m => m.HasAttribute<ExpectedOutputAttribute>());
		}
		private SyntaxNode VisitMemberDeclaration(MemberDeclarationSyntax node, SyntaxNode newNode)
		{
			var newMember = ((MemberDeclarationSyntax) newNode).WithoutAttributes();
			var isSolutionPart = node.HasAttribute<ExcludeFromSolutionAttribute>() || node.HasAttribute<ExerciseAttribute>();
			if (node.HasAttribute<ExcludeFromSolutionAttribute>())
				Slide.EthalonSolution += newMember.ToFullString();
			return isSolutionPart ? null : newMember;
		}


		public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			return VisitMemberDeclaration(node, base.VisitClassDeclaration(node));
		}

		public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
		{
			return VisitMemberDeclaration(node, base.VisitEnumDeclaration(node));
		}

		public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
		{
			return VisitMemberDeclaration(node, base.VisitConstructorDeclaration(node));
		}

		public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
		{
			return VisitMemberDeclaration(node, base.VisitFieldDeclaration(node));
		}

		public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
		{
			return VisitMemberDeclaration(node, base.VisitPropertyDeclaration(node));
		}

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			var newMethod = VisitMemberDeclaration(node, base.VisitMethodDeclaration(node));
			if (node.HasAttribute<ExpectedOutputAttribute>())
			{
				if (ExerciseClassName == null)
					ExerciseClassName = GetParentCassName(node);

				Slide.ExpectedOutput = node.GetAttributes<ExpectedOutputAttribute>().Select(attr => attr.GetArgument(0)).FirstOrDefault();
			}
			if (node.HasAttribute<HideExpectedOutputOnErrorAttribute>())
				Slide.HideExpectedOutputOnError = true;
			if (node.HasAttribute<HintAttribute>())
				Slide.HintsMd.AddRange(node.GetAttributes<HintAttribute>().Select(attr => attr.GetArgument(0)));
			if (node.HasAttribute<CommentAfterExerciseIsSolved>())
				Slide.CommentAfterExerciseIsSolved = node.GetAttributes<CommentAfterExerciseIsSolved>().Single().GetArgument(0);
			if (node.HasAttribute<ExerciseAttribute>())
			{
				ExerciseClassName = GetParentCassName(node);
				Slide.EthalonSolution += node.WithoutAttributes();
				Slide.ExerciseInitialCode = GetExerciseCode(node);
				if (node.HasAttribute<IsStaticMethodAttribute>()) validator.AddValidator(new IsStaticMethodAttribute());
				if (node.HasAttribute<SingleStatementMethodAttribute>()) validator.AddValidator(new SingleStatementMethodAttribute());
			}
			return newMethod;
		}

		private static string GetParentCassName(MethodDeclarationSyntax node)
		{
			return node.GetParents().OfType<ClassDeclarationSyntax>().First().Identifier.Text;
		}

		private string GetExerciseCode(MethodDeclarationSyntax method)
		{
			var codeLines = method.TransformExercise().ToPrettyString().SplitToLines();
			return string.Join("\n", FilterSpecialComments(codeLines));
		}

		private IEnumerable<string> FilterSpecialComments(IEnumerable<string> lines)
		{
			var inUncomment = false;
			foreach (var line in lines)
			{
				if (!inUncomment && line.Trim() == "/*uncomment")
				{
					inUncomment = true;
				}
				else if (inUncomment && line.Trim() == "*/")
				{
					inUncomment = false;
				}
				else
					yield return line;
			}
		}
	}
}