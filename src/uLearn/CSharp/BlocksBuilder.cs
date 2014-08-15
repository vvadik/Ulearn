using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public class SlideBuilder : CSharpSyntaxRewriter
	{
		private readonly Func<string, string> getInclude;
		public readonly List<SlideBlock> Blocks = new List<SlideBlock>();
		public string Title;
		public string Id;

		public SlideBuilder(Func<string, string> getInclude) : base(false)
		{
			this.getInclude = getInclude;
		}

		private SyntaxNode VisitMemberDeclaration(MemberDeclarationSyntax node, SyntaxNode newNode)
		{
			var parent = node.GetParents().OfType<BaseTypeDeclarationSyntax>().FirstOrDefault();
			if (!ShowOnSlide(node)) return null;
			if (parent.HasAttribute<SlideAttribute>()) AddCodeBlock(((MemberDeclarationSyntax)newNode));
			return ((MemberDeclarationSyntax)newNode).WithoutAttributes();
		}

		public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			if (node.HasAttribute<SlideAttribute>())
			{
				var arguments =
					node.GetAttributes<SlideAttribute>()
					.Select(a => new { title = a.GetArgument(0), id = a.GetArgument(1) })
					.Single();
				Title = arguments.title;
				Id = arguments.id;
			}
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

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			return VisitMemberDeclaration(node, base.VisitMethodDeclaration(node));
		}

		public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
		{
			var comment = trivia.ToString();
			if (trivia.CSharpKind() == SyntaxKind.MultiLineCommentTrivia)
			{
				bool shouldCreateTextBlock = trivia.GetParents().Count(p => IsNestingParent(p, trivia)) <= 1;
				if (shouldCreateTextBlock)
					Blocks.Add(ExtractMarkDownFromComment(trivia));
			}
			else if (trivia.CSharpKind() == SyntaxKind.SingleLineCommentTrivia)
			{
				if (comment.StartsWith("//#"))
				{
					var parts = comment.Split(new[] { ' ' }, 2);
					if (parts[0] == "//#video") EmbedVideo(parts[1]);
					if (parts[0] == "//#include") EmbedCode(parts[1]);
				}
			}
			return base.VisitTrivia(trivia);
		}

		///<summary>Is child _inside_ Type or Method parent</summary>
		private bool IsNestingParent(SyntaxNode parent, SyntaxTrivia child)
		{
			return IsNestingParent(parent as TypeDeclarationSyntax, child) 
				|| IsNestingParent(parent as MethodDeclarationSyntax, child);
		}

		private bool IsNestingParent(TypeDeclarationSyntax node, SyntaxTrivia trivia)
		{
			if (node == null) return false;
			if (trivia.Span.Start < node.OpenBraceToken.Span.Start) return false;
			if (trivia.Span.End > node.CloseBraceToken.Span.End) return false;
			return true;
		}
		
		private bool IsNestingParent(MethodDeclarationSyntax node, SyntaxTrivia trivia)
		{
			if (node == null) return false;
			if (trivia.Span.Start < node.Body.Span.Start) return false;
			if (trivia.Span.End > node.Body.Span.End) return false;
			return true;
		}

		private void AddCodeBlock(MemberDeclarationSyntax node)
		{
			var code = (string)CreateCodeBlock((dynamic)node);
			var lastBlock = Blocks.LastOrDefault() as CodeBlock;
			if (lastBlock != null)
				Blocks[Blocks.Count - 1] = new CodeBlock(lastBlock.Code + "\r\n\r\n" + code);
			else
				Blocks.Add(new CodeBlock(code));
		}

		private string CreateCodeBlock(MethodDeclarationSyntax node)
		{
			return node.HasAttribute<ShowBodyOnSlideAttribute>()
				? node.Body.Statements.ToFullString().RemoveCommonNesting()
				: node.WithoutAttributes().ToPrettyString();
		}

		private string CreateCodeBlock(MemberDeclarationSyntax node)
		{
			return node.WithoutAttributes().ToPrettyString();
		}
		
		private static bool ShowOnSlide(MemberDeclarationSyntax node)
		{
			return !node.HasAttribute<SlideAttribute>()
			&& !node.HasAttribute<HideOnSlideAttribute>() 
			&& !node.HasAttribute<ExerciseAttribute>();
		}

		private void EmbedCode(string filename)
		{
			Blocks.Add(new CodeBlock(getInclude(filename)));
		}

		private void EmbedVideo(string videoId)
		{
			Blocks.Add(new YoutubeBlock(videoId));
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
			return new MdBlock(sb.ToString());
		}
	}
}