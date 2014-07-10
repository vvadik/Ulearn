using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class SlideWalker : CSharpSyntaxRewriter
	{
		public readonly List<SlideBlock> Blocks = new List<SlideBlock>();
		public string ExerciseInitialCode { get; private set; }
		public bool IsExercise { get; private set; }
		public string ExpectedOutput { get; private set; }
		public readonly List<string> Hints = new List<string>();
		public MethodDeclarationSyntax ExerciseNode;

		public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			var classDeclaration = base.VisitClassDeclaration(node);
			if (ShowOnSlide(node))
				AddCodeBlock(node);
			return classDeclaration;
		}

		private void AddCodeBlock(MemberDeclarationSyntax node)
		{
			var sampleBlock = (SlideBlock)CreateSampleBlock((dynamic)node);
			SlideBlock lastBlock = Blocks.LastOrDefault();
			if (lastBlock != null && lastBlock.IsCode)
				Blocks[Blocks.Count - 1] = SlideBlock.FromCode(lastBlock.Text + "\r\n\r\n" + sampleBlock.Text);
			else
				Blocks.Add(sampleBlock);
		}

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			var newMethod = (MethodDeclarationSyntax)base.VisitMethodDeclaration(node);
			if (node.HasAttribute<ExpectedOutputAttribute>())
			{
				IsExercise = true;
				ExpectedOutput = node.GetAttributes<ExpectedOutputAttribute>().Select(attr => attr.GetArgument()).FirstOrDefault();
			}
			if (node.HasAttribute<HintAttribute>())
			{
				Hints.AddRange(node.GetAttributes<HintAttribute>().Select(attr => attr.GetArgument()));
			}

			if (ShowOnSlide(node))
			{
				AddCodeBlock(node);
			}
			else
			{
				if (node.HasAttribute<ExerciseAttribute>())
				{
					ExerciseNode = node;
					ExerciseInitialCode = GetExerciseCode(node);
				}
				return null;
			}
			return newMethod.WithoutAttributes();
		}

		private static bool ShowOnSlide(MemberDeclarationSyntax node)
		{
			return node.HasAttribute<ShowBodyOnSlideAttribute>() || node.HasAttribute<ShowOnSlideAttribute>();
		}

		private SlideBlock CreateSampleBlock(MethodDeclarationSyntax node)
		{
			var code = node.HasAttribute<ShowOnSlideAttribute>() ? node.WithoutAttributes().ToPrettyString() : node.Body.Statements.ToFullString();
			return SlideBlock.FromCode(code.RemoveCommonNesting());
		}

		private SlideBlock CreateSampleBlock(ClassDeclarationSyntax node)
		{
			string code = node.WithAttributeLists(new SyntaxList<AttributeListSyntax>()).ToFullString().RemoveCommonNesting();
			return SlideBlock.FromCode(code);
		}

		private string GetExerciseCode(MethodDeclarationSyntax method)
		{
			var codeLines = method.TransformExercise().ToPrettyString().SplitToLines().RemoveCommonNesting();

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

		public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
		{
			if (trivia.CSharpKind() == SyntaxKind.MultiLineCommentTrivia)
			{
				var firstLine = trivia.ToString().SplitToLines().First().Trim();
				if (firstLine != "/*uncomment")
					Blocks.Add(ExtractMarkDownFromComment(trivia));
			}
			return base.VisitTrivia(trivia);
		}

		public static SlideBlock ExtractMarkDownFromComment(SyntaxTrivia comment)
		{
			int identation = comment.SyntaxTree.GetLineSpan(comment.FullSpan).StartLinePosition.Character;
			string[] commentLines = comment.ToString().SplitToLines();
			var sb = new StringBuilder();
			foreach (string line in commentLines.Skip(1).Take(commentLines.Length - 2))
			{
				if (line.Trim() != "")
				{
					if (line.Length < identation || line.Substring(0, identation).Trim() != "")
						throw new Exception("Wrong identation in line: " + line);
					sb.AppendLine(line.Substring(identation));
				}
				else
				{
					sb.AppendLine();
				}
			}
			return SlideBlock.FromMarkdown(sb.ToString());
		}
	}
}