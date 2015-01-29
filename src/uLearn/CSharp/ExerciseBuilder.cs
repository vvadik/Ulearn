using System.Collections.Generic;
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

		public ExerciseBuilder(string prelude, SlideBuilder blocksBuilder, SlideInfo slideInfo)
			: base(false)
		{
			this.prelude = prelude;
			Slide = new ExerciseSlide(SlideBuilder.LangId, blocksBuilder.Blocks, slideInfo, blocksBuilder.Title, blocksBuilder.Id);
			Slide.ValidatorName = "csharp";
		}

		public ExerciseSlide BuildFrom(SyntaxTree tree)
		{
			ExerciseClassName = null;
			Slide.ExerciseInitialCode = GetUncomment(tree.GetRoot()) ?? ""; //for uncomment-comment without exercise method
			SyntaxNode result = Visit(tree.GetRoot());
			var exerciseInsertIndex = GetExerciseInsertIndex(result);
			const string pragma = "\n#line 1\n";
			Slide.ExerciseCode = prelude + result.ToFullString().Insert(exerciseInsertIndex, pragma);
			Slide.IndexToInsertSolution = prelude.Length + exerciseInsertIndex + pragma.Length;
			return Slide;
		}

		const string uncommentPrefix = "/*uncomment";

		private string GetUncomment(SyntaxNode root)
		{
			var comment = 
				from c in root.DescendantTrivia()
				where c.CSharpKind() == SyntaxKind.MultiLineCommentTrivia
				where c.ToString().StartsWith(uncommentPrefix)
				select GetUncommentBody(c);
			return comment.FirstOrDefault();
		}

		private static string GetUncommentBody(SyntaxTrivia c)
		{
			var comment = c.ToString().Substring(uncommentPrefix.Length);
			return comment.Substring(0, comment.Length - 2).RemoveCommonNesting();
		}

		private int GetExerciseInsertIndex(SyntaxNode tree)
		{
			ClassDeclarationSyntax exerciseClass =
				tree.DescendantNodes()
					.OfType<ClassDeclarationSyntax>()
					.FirstOrDefault(n => n.Identifier.Text == ExerciseClassName);
			if (exerciseClass !=null)
				return exerciseClass.OpenBraceToken.Span.End;
			var ns = tree.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
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
			if (node.HasAttribute<ExcludeFromSolutionAttribute>() 
				|| (node is TypeDeclarationSyntax && node.HasAttribute<ExerciseAttribute>()))
				Slide.EthalonSolution += newMember.ToFullString();
			return isSolutionPart ? null : newMember;
		}


		public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			if (node.HasAttribute<ExerciseAttribute>())
				ExerciseClassName = FindParentClassName(node);
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
				ExerciseClassName = ExerciseClassName ?? FindParentClassName(node);
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
				ExerciseClassName = ExerciseClassName ?? FindParentClassName(node);
				Slide.EthalonSolution += node.WithoutAttributes();
				Slide.ExerciseInitialCode = GetExerciseCode(node);
				if (node.HasAttribute<SingleStatementMethodAttribute>())
					Slide.ValidatorName += " SingleStatementMethod";
			}
			return newMethod;
		}

		private static string FindParentClassName(SyntaxNode node)
		{
			var parent = node.GetParents().OfType<ClassDeclarationSyntax>().FirstOrDefault();
			return parent == null ? null : parent.Identifier.Text;
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