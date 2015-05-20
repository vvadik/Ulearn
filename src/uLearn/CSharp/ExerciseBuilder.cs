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

		public ExerciseBlock Exercise { get; private set; }

		public ExerciseBuilder(string langId, string prelude)
			: base(false)
		{
			this.prelude = prelude;
			Exercise = new ExerciseBlock();
			Exercise.ValidatorName = "cs";
			Exercise.Lang = langId;
		}

		public ExerciseBlock BuildBlockFrom(SyntaxTree tree)
		{
			ExerciseClassName = null;
			Exercise.ExerciseInitialCode = GetUncomment(tree.GetRoot()) ?? ""; //for uncomment-comment without exercise method
			SyntaxNode result = Visit(tree.GetRoot());
			var exerciseInsertIndex = GetExerciseInsertIndex(result);
			const string pragma = "\n#line 1\n";
			Exercise.ExerciseCode = prelude + result.ToFullString().Insert(exerciseInsertIndex, pragma);
			Exercise.IndexToInsertSolution = prelude.Length + exerciseInsertIndex + pragma.Length;
			return Exercise;
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
				Exercise.EthalonSolution += newMember.ToFullString();
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

		public override SyntaxNode VisitDelegateDeclaration(DelegateDeclarationSyntax node)
		{
			return VisitMemberDeclaration(node, base.VisitDelegateDeclaration(node));
		}

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			var newMethod = VisitMemberDeclaration(node, base.VisitMethodDeclaration(node));
			if (node.HasAttribute<ExpectedOutputAttribute>())
			{
				ExerciseClassName = ExerciseClassName ?? FindParentClassName(node);
				Exercise.ExpectedOutput = node.GetAttributes<ExpectedOutputAttribute>().Select(attr => attr.GetArgument(0)).FirstOrDefault();
			}
			if (node.HasAttribute<HideExpectedOutputOnErrorAttribute>())
				Exercise.HideExpectedOutputOnError = true;
			if (node.HasAttribute<HintAttribute>())
				Exercise.HintsMd.AddRange(node.GetAttributes<HintAttribute>().Select(attr => attr.GetArgument(0)));
			if (node.HasAttribute<CommentAfterExerciseIsSolved>())
				Exercise.CommentAfterExerciseIsSolved = node.GetAttributes<CommentAfterExerciseIsSolved>().Single().GetArgument(0);
			if (node.HasAttribute<ExerciseAttribute>())
			{
				ExerciseClassName = ExerciseClassName ?? FindParentClassName(node);
				Exercise.EthalonSolution += node.WithoutAttributes();
				Exercise.ExerciseInitialCode = GetExerciseCode(node);
				if (node.HasAttribute<SingleStatementMethodAttribute>())
					Exercise.ValidatorName += " SingleStatementMethod";
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